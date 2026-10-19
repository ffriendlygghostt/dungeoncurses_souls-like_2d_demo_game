using UnityEngine;

public class Freezable : MonoBehaviour
{
    public float duration = 7f;
    public GameObject freezeEffect;

    private float chillTimer = 0f;
    private float freezeTimer = 0f;

    private bool isChilled = false;
    private bool isFrozen = false;

    private IMove mover;
    private Burnable burnable;
    private Bleedable bleedable;
    private Rotable rotable;
    private DamageVisuals damageVisuals;
    private HealingBook healingBook;
    private IEnemyAttack attack;
    private IEnemyMovement movement;
    private PlayerAttack playerAttack;
    private Rigidbody2D rg;
    private float defaultMass;

    public bool IsFrozen => isFrozen;
    public bool IsChilled => isChilled;

    void Awake()
    {
        mover = GetComponent<IMove>();
        burnable = GetComponent<Burnable>();
        bleedable = GetComponent<Bleedable>();
        damageVisuals = GetComponent<DamageVisuals>();
        healingBook = GetComponent<HealingBook>();
        playerAttack = GetComponent<PlayerAttack>();

        attack = GetComponent<IEnemyAttack>();
        movement = GetComponent<IEnemyMovement>();

        rotable = GetComponent<Rotable>();
        rg = GetComponent<Rigidbody2D>();
        if (rg != null ) 
            defaultMass = rg.mass;
    }

    void Update()
    {
        if (isFrozen)
        {
            freezeTimer -= Time.deltaTime;
            if (freezeTimer <= 0f)
                RemoveFreeze();

        }
        else if (isChilled)
        {
            chillTimer -= Time.deltaTime;
            if (chillTimer <= 0f)
                RemoveChill();
        }
    }

    public void ApplyChill(float? customDuration = null)
    {
        float finalDuration = customDuration ?? duration;

        if (isFrozen)
            return;


        if (rotable != null && rotable.IsRotting)
        {
            rotable.TriggerRotExplosion(50); 
            return;
        }

        if (bleedable != null && bleedable.IsBleeding)
        {
            bleedable.StopBleeding();
            StartFreeze(finalDuration);
            return;
        }

        if (burnable != null && burnable.IsBurning)
        {
            burnable.Extinguish();
        }

        if (isChilled)
        {
            RemoveChill();
            StartFreeze(finalDuration);
            return;
        }

        StartChill(finalDuration);
    }


    private void StartFreeze(float? customDuration = null)
    {
        isFrozen = true;
        freezeTimer = duration;

        if(healingBook!=null) healingBook.CantHeal();
        if(playerAttack!=null) playerAttack.CanAttack(false);
        if (attack != null) attack.CanAttack(false);

        burnable?.Extinguish();
        bleedable?.StopBleeding();

        mover?.FreezeMovement(true);
        movement?.FreezeMovement(true);
        damageVisuals?.ShowEffect(DamageVisuals.EffectType.Freeze, duration);
            if (rg != null)
                rg.constraints = RigidbodyConstraints2D.FreezeAll;
        if (freezeEffect != null)
            freezeEffect.SetActive(true);
    }

    public void StartFreez(float? customDuration = null)
    {
        StartFreeze(customDuration);
    }
    private void RemoveFreeze()
    {
        if (!isFrozen) return;

        isFrozen = false;
        if(healingBook!=null) healingBook.CanHeal();
        if (playerAttack != null) playerAttack.CanAttack(true);
        if (attack != null) attack.CanAttack(true);


        mover?.FreezeMovement(false);
        movement?.FreezeMovement(false);

        if (freezeEffect != null)
            freezeEffect.SetActive(false);

        if (rg != null)
            rg.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    public void RemoveFreezeP()
    {
        RemoveFreeze();
    }

    private void StartChill(float? customDuration = null)
    {
        isChilled = true;
        chillTimer = duration;

        burnable?.Extinguish();
        mover?.SetSpeedMultiplier(0.5f);
        mover?.SetDashMultiplier(0.5f);

        movement?.SetSpeedMultiplier(0.5f);

        damageVisuals?.ShowEffect(DamageVisuals.EffectType.Cooling, duration);
    }

    private void RemoveChill()
    {
        if (!isChilled) return;

        isChilled = false;

        mover?.SetSpeedMultiplier(1f);
        mover?.SetDashMultiplier(1f);

        movement?.SetSpeedMultiplier(1f);
    }

    public void RemoveChillP()
    {
        RemoveChill();
    }
}
