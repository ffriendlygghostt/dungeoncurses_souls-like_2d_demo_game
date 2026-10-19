using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Candle : MonoBehaviour
{
    public float reigniteDelay = 15f;

    public float candleAnimationSpeed = 1.0f;
    public float lightAnimationSpeed = 1.0f;
    public float backLightAnimationSpeed = 1.0f;

    private Animator candleAnimator;
    private Light2D candleLight;
    private Animator lightAnimator;
    private Animator backLightAnimator;

    private GameObject backLightObject;

    private Coroutine reigniteRoutine;

    private bool playerInside = false;
    private Burnable playerBurnable = null;


    void Start()
    {
        candleAnimator = GetComponent<Animator>();
        candleLight = transform.Find("Light")?.GetComponent<Light2D>();
        backLightObject = transform.Find("BackLight")?.gameObject;

        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            if (anim == candleAnimator)
                continue;

            if (anim.gameObject.name == "Light")
                lightAnimator = anim;
            else if (anim.gameObject.name == "BackLight")
                backLightAnimator = anim;
        }

        if (candleAnimator != null)
            candleAnimator.speed = candleAnimationSpeed;

        if (lightAnimator != null)
            lightAnimator.speed = lightAnimationSpeed;

        if (backLightAnimator != null)
            backLightAnimator.speed = backLightAnimationSpeed;

        IgniteNow();
    }
    void Update()
    {
        if (playerInside && playerBurnable != null && playerBurnable.IsBurning)
        {
            if (!candleLight.enabled)
            {
                if (reigniteRoutine != null)
                    StopCoroutine(reigniteRoutine);

                IgniteNow();
            }
        }
    }

    public void Smoking()
    {
        if (reigniteRoutine != null)
            StopCoroutine(reigniteRoutine);

        if (candleAnimator != null)
            candleAnimator.SetBool("IsOn", false);

        if (candleLight != null)
            candleLight.enabled = false;
        else
            Debug.Log("CandleLight is null");

        if (backLightObject != null)
            backLightObject.SetActive(false);

        reigniteRoutine = StartCoroutine(ReigniteAfterDelay());
    }

    public void IgniteNow()
    {
        if (candleAnimator != null)
            candleAnimator.SetBool("IsOn", true);

        if (candleLight != null)
            candleLight.enabled = true;

        if (backLightObject != null)
            backLightObject.SetActive(true);

        if (lightAnimator != null)
            lightAnimator.speed = lightAnimationSpeed;

        if (backLightAnimator != null)
            backLightAnimator.speed = backLightAnimationSpeed;

        if (candleAnimator != null)
            candleAnimator.speed = candleAnimationSpeed;
    }

    private IEnumerator ReigniteAfterDelay()
    {
        yield return new WaitForSeconds(reigniteDelay);
        IgniteNow();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerFullTrigger"))
        {
            playerBurnable = other.GetComponentInParent<Burnable>();
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerFullTrigger"))
        {
            if (other.GetComponentInParent<Burnable>() == playerBurnable)
            {
                playerInside = false;
                playerBurnable = null;
            }
        }
    }


}
