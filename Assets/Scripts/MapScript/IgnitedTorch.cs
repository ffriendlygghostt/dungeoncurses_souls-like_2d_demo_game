using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class IgnitedTorch : MonoBehaviour
{
    public float reigniteDelay = 15f;
    public float candleAnimationSpeed = 1.0f;
    public float lightAnimationSpeed = 1.0f;
    public float backlightAnimationSpeed = 1.0f;

    private Animator candleAnimator;
    private Light2D candleLight;
    private Animator lightAnimator;

    private Animator backlightAnimator; 

    private Coroutine reigniteRoutine;

    private bool playerInside = false;
    private Burnable playerBurnable = null;

    [Header("Maybe?")]
    public GameObject backfire;


    void Start()
    {
        candleAnimator = GetComponent<Animator>();
        candleLight = transform.Find("Light")?.GetComponent<Light2D>();

        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var anim in animators)
        {
            if (anim == candleAnimator)
                continue;

            if (anim.gameObject.name == "Light")
                lightAnimator = anim;

            if (anim.gameObject.name == "BackLight")
                backlightAnimator = anim; 
        }

      
        if (candleAnimator != null)
            candleAnimator.speed = candleAnimationSpeed;

        if (lightAnimator != null)
            lightAnimator.speed = lightAnimationSpeed;

        if (backlightAnimator != null)
            backlightAnimator.speed = backlightAnimationSpeed;

        candleLight.enabled = true;
        candleAnimator.SetBool("IsOn", true);
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

        if (backfire != null)
            backfire.SetActive(false);

        reigniteRoutine = StartCoroutine(ReigniteAfterDelay());
    }

    public void IgniteNow()
    {
        if (candleAnimator != null)
            candleAnimator.SetBool("IsOn", true);

        if (candleLight != null)
            candleLight.enabled = true;

        if (lightAnimator != null)
            lightAnimator.speed = lightAnimationSpeed;

        if (candleAnimator != null)
            candleAnimator.speed = candleAnimationSpeed;

        if (backfire != null)
            backfire.SetActive(true);
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
