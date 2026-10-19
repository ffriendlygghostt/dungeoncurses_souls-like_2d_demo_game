using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExtinguishedTorch : MonoBehaviour
{
    public float reigniteDelay = 15f;
    public float candleAnimationSpeed = 1.0f;
    public float lightAnimationSpeed = 1.0f;

    private Animator candleAnimator;
    private Animator lightAnimator;
    private Light2D candleLight;

    private Coroutine reigniteRoutine;

    private bool playerInside = false;
    private Burnable playerBurnable = null;

    public GameObject candleLightObject;

    [Header("Optional")]
    public GameObject backfire;
    public float backlightspeed = 1.0f;
    private Animator backlightAnimator;

    private bool isActiveIgnited = false;
    void Start()
    {
        candleAnimator = GetComponent<Animator>();
        if (candleLightObject != null)
        {
            candleLightObject.SetActive(true);
            candleLight = candleLightObject.GetComponent<Light2D>();
            if (candleLight != null)
                candleLight.enabled = false;
        }



        Animator[] animators = GetComponentsInChildren<Animator>(true);
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
            backlightAnimator.speed = backlightspeed;

        TurnOffCompletely();
    }

    void Update()
    {
        if (playerInside && playerBurnable != null && playerBurnable.IsBurning)
        {
            if (!candleLight.enabled && !isActiveIgnited)
            {
                IgniteNow();
            }

            if (isActiveIgnited && reigniteRoutine != null)
            {
                StopCoroutine(reigniteRoutine);
                reigniteRoutine = null;
            }
        }
        else if (isActiveIgnited && reigniteRoutine == null)
        {
            reigniteRoutine = StartCoroutine(ReigniteCountdown());
        }
    }

    public void Smoking()
    {
        if (reigniteRoutine != null)
        {
            StopCoroutine(reigniteRoutine);
            reigniteRoutine = null;
        }

        TurnOffCompletely();
    }

    public void IgniteNow()
    {
        isActiveIgnited = true;

        if (candleAnimator != null)
            candleAnimator.SetBool("IsOn", true);

        if (candleLight != null)
            candleLight.enabled = true;

        if (backfire != null)
            backfire.SetActive(true);
    }

    private void TurnOffCompletely()
    {
        isActiveIgnited = false;

        if (candleAnimator != null)
            candleAnimator.SetBool("IsOn", false);

        if (candleLight != null)
            candleLight.enabled = false;

        if (backfire != null)
            backfire.SetActive(false);
    }

    private IEnumerator ReigniteCountdown()
    {
        yield return new WaitForSeconds(reigniteDelay);
        TurnOffCompletely();
        reigniteRoutine = null;
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
