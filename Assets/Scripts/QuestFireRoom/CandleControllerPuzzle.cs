using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CandleControllerPuzzle : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public int candleIndex;

    [Header("Visual Components")]
    public Light2D candleLight;
    public Animator candleAnimator;
    public GameObject backfire;

    private bool isLit = true;
    private bool hasReported = false;

    public bool IsLit => isLit;

    void Start()
    {
        if (candleAnimator == null)
            candleAnimator = GetComponent<Animator>();
        if (candleLight == null)
            candleLight = transform.Find("Light")?.GetComponent<Light2D>();

        SetLitState(true);
    }

    public void Extinguish()
    {
        if (!isLit) return;

        SetLitState(false);

        if (!hasReported)
        {
            FindObjectOfType<SecretDoorPuzzle>()?.OnCandleExtinguished(candleIndex);
            hasReported = true;
        }
    }

    public void Ignite()
    {
        SetLitState(true);
        hasReported = false;
    }

    private void SetLitState(bool lit)
    {
        isLit = lit;

        if (candleAnimator != null)
            candleAnimator.SetBool("IsOn", lit);

        if (candleLight != null)
            candleLight.enabled = lit;

        if (backfire != null)
            backfire.SetActive(lit);
    }

    public void Smoking()
    {
        if (IsLit)
            Extinguish();
    }

}

