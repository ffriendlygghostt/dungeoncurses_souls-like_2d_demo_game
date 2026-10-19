using System.Collections;
using UnityEngine;

public class Rotable : MonoBehaviour, IRoting
{
    [Header("Default Rot Settings")]
    public int defaultTickDamage = 4;
    public float defaultTickInterval = 3f;
    public float defaultDuration = 15f;

    [Header("Effect")]
    public GameObject poisonEffect;

    [Header("Spawned on Death")]
    public GameObject rotPuddlePrefab;


    public bool IsRotting { get; private set; }


    private int currentTicks = 0;
    private Coroutine rotCoroutine;
    private IHealth health;
    private DamageVisuals damageVisuals;
    private HealingBook healingBook;
    private Burnable burnable;
    private Bleedable bleedable;
    private Freezable freezable;

    private int activeTickDamage;
    private float activeTickInterval;
    private int defaultMultiplier = 100;

    private bool isDead => health != null && health.IsDead;

    private void Awake()
    {
        health = GetComponent<IHealth>();
        damageVisuals = GetComponent<DamageVisuals>();
        healingBook = GetComponent<HealingBook>();
        burnable = GetComponent<Burnable>();
        bleedable = GetComponent<Bleedable>();
        freezable = GetComponent<Freezable>();
    }

    public void ApplyRot(float duration = -1f, float tickInterval = -1f, int tickDamage = -1)
    {
        if (health == null)
            return;

        if (isDead)
        {
            SpawnRotPuddle();
            return;
        }

        if (burnable != null && burnable.IsBurning)
        {
            burnable.ExtendBurnDuration(0.5f);
            return;
        }

        if (freezable != null && (freezable.IsChilled || freezable.IsFrozen))
            return;

        if (poisonEffect != null)
        {
            poisonEffect.SetActive(true);
            StartCoroutine(DeactivateAfter(poisonEffect, 1f));
        }

        float finalDuration = duration > 0f ? duration : defaultDuration;
        float finalInterval = tickInterval > 0f ? tickInterval : defaultTickInterval;
        int finalDamage = tickDamage > 0 ? tickDamage : defaultTickDamage;

        int tickCount = Mathf.CeilToInt(finalDuration / finalInterval);
        if (tickCount <= 0)
            return;

        bool wasNotRotting = currentTicks <= 0;

        // Если параметры совпадают с текущими, просто прибавляем тики и не сбрасываем корутину
        bool parametersMatch = Mathf.Approximately(activeTickInterval, finalInterval)
                               && activeTickDamage == finalDamage;

        if (bleedable != null && bleedable.IsBleeding)
        {
            // При кровотечении всегда стекать (наращивать) текущие тики
            currentTicks += tickCount;
        }
        else
        {
            if (!parametersMatch)
            {
                // Параметры изменились — обновляем и перезапускаем корутину
                currentTicks = tickCount;
                activeTickInterval = finalInterval;
                activeTickDamage = finalDamage;

                if (rotCoroutine != null)
                {
                    StopCoroutine(rotCoroutine);
                    rotCoroutine = null;
                }
            }
            else
            {
                // Параметры те же — просто добавляем тики (стекаем)
                currentTicks += tickCount;
            }
        }

        healingBook?.CantHeal();
        Debug.Log("HEAL BLOCKED");

        if (wasNotRotting || rotCoroutine == null)
        {
            IsRotting = true;
            rotCoroutine = StartCoroutine(RotRoutine());
        }
    }



    private IEnumerator RotRoutine()
    {
        float timer = 0f;

        while (currentTicks > 0)
        {
            timer += Time.deltaTime;

            if (timer >= activeTickInterval)
            {
                timer = 0f;
                currentTicks--;

                bool wasAliveBefore = !isDead;

                health?.ApplyDamage(activeTickDamage);

                if (!isDead && wasAliveBefore)
                {
                    damageVisuals?.ShowEffect(DamageVisuals.EffectType.Rot);
                    if (poisonEffect != null)
                    {
                        poisonEffect.SetActive(true);
                        StartCoroutine(DeactivateAfter(poisonEffect, 1f));
                    }
                }
                else if (isDead && wasAliveBefore)
                {
                    SpawnRotPuddle();
                }
            }

            yield return null;
        }

        rotCoroutine = null;
        IsRotting = false;
        healingBook?.CanHeal();
    }

    private IEnumerator DeactivateAfter(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj?.SetActive(false);
    }

    public void StopRot()
    {
        currentTicks = 0;
        IsRotting = false;
        healingBook?.CanHeal();

        if (rotCoroutine != null)
        {
            StopCoroutine(rotCoroutine);
            rotCoroutine = null;
        }

        if (poisonEffect != null)
            poisonEffect.SetActive(false);
    }

    public void TriggerRotExplosion(int? explosionMult = null)
    {
        bool wasAliveBefore = !isDead;

        int multiplier = explosionMult ?? defaultMultiplier;
        int totalDamage = currentTicks * activeTickDamage * multiplier / 100;

        health?.ApplyDamage(totalDamage);

        if (wasAliveBefore && isDead)
        {
            SpawnRotPuddle();
        }

        StopRot();
    }

    private void SpawnRotPuddle()
    {
        if (rotPuddlePrefab != null)
        {
            Instantiate(rotPuddlePrefab, transform.position, Quaternion.identity);
        }
    }
}
