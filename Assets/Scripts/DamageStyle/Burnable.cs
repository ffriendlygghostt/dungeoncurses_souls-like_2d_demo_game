using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Burnable : MonoBehaviour, IIgnitable, IBurnReaction
{
    [Header("Burn Settings")]
    public float burnDuration = 7.5f;
    public float burnTickInterval = 1.5f;
    public int damagePerTick = 6;


    [Header("Steam Effect")]
    public GameObject steamEffect;
    public float steamDamageMultiplier = 1f;

    public bool IsBurning { get; private set; }

    private float burnTimer;
    private float tickTimer;

    private IHealth health;
    private Coroutine flashCoroutine;
    private DamageVisuals damageVisuals;
    private Freezable freezable;

    private bool wasBleedingOnIgnite = false;
    private bool wasRottingOnIgnite = false;

    const float IGNORE = -1f;
    const int IGNOREI = -1;

    private PlayerStats playerStats;
    //zaglushka
    private HPB hpb;  


    void Awake()
    {
        health = GetComponent<IHealth>();
        damageVisuals = GetComponent<DamageVisuals>();
        freezable = GetComponent<Freezable>();
        hpb = FindObjectOfType<HPB>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (!IsBurning)
            return;

        burnTimer -= Time.deltaTime;
        tickTimer -= Time.deltaTime;

        if (tickTimer <= 0f && burnTimer > 0f)
        {
            tickTimer += burnTickInterval;

            float multiplier = 1f;
            if (wasBleedingOnIgnite) multiplier += 0.5f;
            if (wasRottingOnIgnite) multiplier += 0.5f;

            int baseDamage = Mathf.RoundToInt(damagePerTick * multiplier);

            var reaction = GetComponent<IBurnReaction>();
            if (reaction != null && reaction.OnBurnTick(this, ref baseDamage))
            {
                return;
            }

            if (playerStats != null)
                baseDamage = Mathf.RoundToInt(baseDamage * (1f - playerStats.fireResistance));

            

            if (baseDamage > 0f)
            {
                health?.ApplyDamage(baseDamage, new Color(1f, 0.5f, 0f));
                damageVisuals?.ShowEffect(DamageVisuals.EffectType.Burn);
            }
        }

        if (burnTimer <= 0f)
        {
            Extinguish();
        }
    }


    public void Ignite()
    {
        Ignite(null);
    }

    public void Ignite(GameObject source, float duration = IGNORE, float tickInterval = IGNORE, int tickDamage = IGNOREI)
    {
        if (freezable != null && freezable.IsFrozen)
        {
            TriggerSteamExplosion();
            return;
        }

        if (freezable != null && freezable.IsChilled)
        {
            freezable.RemoveChillP();
            damageVisuals?.ShowEffect(DamageVisuals.EffectType.Burn);
            Extinguish();
            return;
        }

        if (duration > 0f) burnDuration = duration;
        if (tickInterval > 0f) burnTickInterval = tickInterval;
        if (tickDamage >= 0) damagePerTick = tickDamage;

        if (!IsBurning || burnDuration >= burnTimer)
        {
            IsBurning = true;
            burnTimer = burnDuration;
            tickTimer = burnTickInterval;

            var bleedable = GetComponent<Bleedable>();
            wasBleedingOnIgnite = bleedable != null && bleedable.IsBleeding;

            var rotable = GetComponent<Rotable>();
            wasRottingOnIgnite = rotable != null && rotable.IsRotting;

            if (fireEffectObject != null)
                fireEffectObject.SetActive(true);
        }
    }



    private void TriggerSteamExplosion()
    {
        if (steamEffect != null)
        {
            steamEffect.SetActive(true);
            StartCoroutine(DeactivateAfter(steamEffect, 0.8f));
        }

        int totalTicks = Mathf.CeilToInt(burnDuration / burnTickInterval);
        int totalDamage = Mathf.RoundToInt(damagePerTick * totalTicks * steamDamageMultiplier);

        health?.ApplyDamage(totalDamage, new Color(1f, 0.5f, 0f));
        damageVisuals.ShowEffect(DamageVisuals.EffectType.Burn);
        freezable.RemoveFreezeP();

        Extinguish();
    }


    private IEnumerator DeactivateAfter(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void Extinguish()
    {
        IsBurning = false;

        if (fireEffectObject != null)
            fireEffectObject.SetActive(false);

        if (wasBleedingOnIgnite)
        {
            var bleedable = GetComponent<Bleedable>();
            bleedable?.StopBleeding();
        }

        if (wasRottingOnIgnite)
        {
            var rotable = GetComponent<Rotable>();
            rotable?.StopRot();
        }

        wasBleedingOnIgnite = false;
        wasRottingOnIgnite = false;
    }

    public void TriggerPartialBurnExplosion(float fraction)
    {
        int totalTicks = Mathf.CeilToInt(burnTimer / burnTickInterval);
        int ticksToExplode = Mathf.FloorToInt(totalTicks * fraction);
        int damage = Mathf.RoundToInt(damagePerTick * ticksToExplode);

        health?.ApplyDamage(damage, new Color(1f, 0.5f, 0f));
        hpb.ShowHealthDamage(damage);
        damageVisuals?.ShowEffect(DamageVisuals.EffectType.Burn);

        burnTimer -= ticksToExplode * burnTickInterval;
        tickTimer = 0f;
        if (burnTimer <= 0f)
            Extinguish();
    }

    public void ExtendBurnDuration(float multiplier) 
    { 
        if (!IsBurning)
            return;

        burnTimer += burnTimer * multiplier;
    }

    public bool OnBurnTick(Burnable burnable, ref int damage)
    {
        if (health != null && damage > 0)
        {
            health.Heal(damage);
        }
        return true;
    }

    public GameObject fireEffectObject; 
}
