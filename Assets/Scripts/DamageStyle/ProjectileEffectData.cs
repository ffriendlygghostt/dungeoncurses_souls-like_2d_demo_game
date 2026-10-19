using UnityEngine;

public enum EffectType
{
    None,
    Fire,
    Ice,
    Rot,
    Lightning,
    Darkness,
    Madness,
    Fear,
    Blood
}

[CreateAssetMenu(menuName = "Projectile/Effect Data", fileName = "NewProjectileEffect")]
public class ProjectileEffectData : ScriptableObject
{
    public EffectType type;
    public int tickDamage;
    public float tickInterval;
    public float duration;
}