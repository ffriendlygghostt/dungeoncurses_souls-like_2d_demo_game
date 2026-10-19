using System.Collections;
using UnityEngine;

public class Bleedable : MonoBehaviour, IBleeding
{
    public int defaultBleedDamage = 2;
    public float defaultTickInterval = 1f;
    public float defaultDurationBlood = 7.5f;
    public GameObject[] bleedEffects;
    public GameObject rotSteamEffect;

    public bool IsBleeding { get; private set; }

    private int currentTicks = 0;
    private Coroutine bleedCoroutine;

    private Burnable burnable;
    private DamageVisuals damageVisuals;
    private Freezable freezable;
    private Rotable rotable;
    private IHealth health;

    private float activeTickInterval;
    private int activeTickDamage;
    private float triggerPartialBurn = 0.5f;

    void Awake()
    {
        burnable = GetComponent<Burnable>();
        damageVisuals = GetComponent<DamageVisuals>();
        freezable = GetComponent<Freezable>();
        rotable = GetComponent<Rotable>();
        health = GetComponent<IHealth>();
    }

    public void ApplyBleed(float duration = -1f, float tickInterval = -1f, int tickDamage = -1)
    {

        if ((freezable?.IsFrozen ?? false) || (freezable?.IsChilled ?? false))
            return;

        if (burnable != null && burnable.IsBurning)
        {
            burnable.TriggerPartialBurnExplosion(triggerPartialBurn);
            return;
        }

        activeTickInterval = tickInterval > 0f ? tickInterval : defaultTickInterval;
        int incomingDamage = tickDamage >= 0 ? tickDamage : defaultBleedDamage;
        if (incomingDamage > activeTickDamage)
            activeTickDamage = incomingDamage;
        float finalDuration = duration > 0f ? duration : defaultDurationBlood;
        int newTicks = Mathf.CeilToInt(finalDuration / activeTickInterval);


        currentTicks += newTicks;
        bool wasNotBleeding = !IsBleeding;
        IsBleeding = currentTicks > 0;


        if (rotable != null && rotable.IsRotting)
        {
            int totalBleedDamage = currentTicks * activeTickDamage;
            health?.ApplyDamage(totalBleedDamage);
            damageVisuals?.ShowEffect(DamageVisuals.EffectType.Bleed);
            SpawnBleedEffect();

            if (rotSteamEffect != null)
            {
                rotSteamEffect.SetActive(true);
                StartCoroutine(DeactivateAfter(rotSteamEffect, 0.8f));
            }

            rotable.TriggerRotExplosion(); 
            StopBleeding();
            return;
        }

        if (wasNotBleeding && bleedCoroutine == null)
        {
            SpawnBleedEffect();
            bleedCoroutine = StartCoroutine(BleedRoutine());
        }
    }

    private IEnumerator BleedRoutine()
    {
        float tickTimer = 0f;

        while (currentTicks > 0)
        {
            tickTimer += Time.deltaTime;

            if (tickTimer >= activeTickInterval)
            {
                tickTimer = 0f;
                currentTicks--;

                health?.ApplyDamage(activeTickDamage);
                damageVisuals?.ShowEffect(DamageVisuals.EffectType.Bleed);
                SpawnBleedEffect();
            }

            yield return null;
        }

        bleedCoroutine = null;
        IsBleeding = false;
    }

    public void StopBleeding()
    {
        currentTicks = 0;
        IsBleeding = false;

        if (bleedCoroutine != null)
        {
            StopCoroutine(bleedCoroutine);
            bleedCoroutine = null;
        }

        foreach (var eff in bleedEffects)
        {
            if (eff != null)
                eff.SetActive(false);
        }
    }

    private void SpawnBleedEffect()
    {
        if (bleedEffects == null || bleedEffects.Length == 0)
            return;

        int startIndex = Random.Range(0, bleedEffects.Length);

        for (int i = 0; i < bleedEffects.Length; i++)
        {
            int index = (startIndex + i) % bleedEffects.Length;
            GameObject effect = bleedEffects[index];

            if (!effect.activeInHierarchy)
            {
                effect.SetActive(true);
                StartCoroutine(DeactivateAfter(effect, 1f));
                break;
            }
        }
    }

    private IEnumerator DeactivateAfter(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }
}
