using UnityEngine;

public class IceExplosionEffect : MonoBehaviour, IExplosionEffect
{
    private Iceball parentIceball;

    void Start()
    {
        parentIceball = GetComponentInParent<Iceball>();
    }

    public void ApplyEffect(GameObject target)
    {
        var health = target.GetComponent<IHealth>();
        if (health != null)
            health.ApplyDamage(parentIceball.GetInitialDamage());

        var freezable = target.GetComponent<Freezable>();
        if (freezable != null)
        {
            freezable.ApplyChill(parentIceball.GetFreezeDuration());
        }
    }

}
