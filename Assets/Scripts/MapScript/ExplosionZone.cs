using UnityEngine;

public class ExplosionZone : MonoBehaviour
{
    private IExplosionEffect[] explosionEffects;

    void Start()
    {
        explosionEffects = GetComponents<IExplosionEffect>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var effect in explosionEffects)
        {
            effect.ApplyEffect(other.gameObject);
        }
    }

}
