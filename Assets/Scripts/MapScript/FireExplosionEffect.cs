using UnityEngine;

public class FireExplosionEffect : MonoBehaviour, IExplosionEffect
{
    private Fireball parentFireball;


    void Start()
    {
        parentFireball = GetComponentInParent<Fireball>();
    }

    public void ApplyEffect(GameObject target)
    {
        var health = target.GetComponent<IHealth>();
        if (health != null)
            health.ApplyDamage(parentFireball.GetInitialDamage());
        var damageVisual = target.GetComponent<DamageVisuals>();
        if (damageVisual != null)
            damageVisual?.ShowEffect(DamageVisuals.EffectType.Burn);

        var burnable = target.GetComponent<Burnable>();
        if (burnable != null)
        {
            burnable.damagePerTick = parentFireball.GetBurnDamage();
            burnable.burnTickInterval = parentFireball.GetBurnInterval();
            burnable.burnDuration = parentFireball.GetBurnDuration();
            burnable.Ignite(parentFireball.gameObject);
        }
    }
}
