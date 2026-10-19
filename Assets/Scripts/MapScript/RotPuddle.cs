using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotPuddle : MonoBehaviour
{
    [Header("Lifetime Settings")]
    public float activeDuration = 6f; // сколько живёт лужа в активном состоянии
    public float fadeDamageDuration = 1f; // сколько времени ещё дамажит после RotingStop
    public float destroyDelay = 0.42f; // когда уничтожить лужу после конца урона

    [Header("Effect Settings")]
    public int tickDamage = 10;
    public float tickInterval = 2f;
    public float rotChance = 0.4f;
    public ProjectileEffectData rotEffectData;

    private Animator animator;
    private HashSet<GameObject> targetsInPuddle = new HashSet<GameObject>();
    private Dictionary<GameObject, Coroutine> damageCoroutines = new Dictionary<GameObject, Coroutine>();
    private DamageVisuals damageVisuals;
    private GameObject parentObject;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(LifetimeRoutine());

        Transform parentTransform = transform.parent;
        parentObject = parentTransform.gameObject;
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(activeDuration);
        animator.SetTrigger("RotingStop");

        yield return new WaitForSeconds(fadeDamageDuration);

        foreach (var pair in damageCoroutines)
        {
            if (pair.Value != null)
                StopCoroutine(pair.Value);
        }

        yield return new WaitForSeconds(destroyDelay);
        Destroy(parentObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<IHealth>(out var health))
            return;

        if (!targetsInPuddle.Contains(other.gameObject))
        {
            targetsInPuddle.Add(other.gameObject);
            var coroutine = StartCoroutine(DamageRoutine(other.gameObject, health));
            damageCoroutines[other.gameObject] = coroutine;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (targetsInPuddle.Contains(other.gameObject))
        {
            targetsInPuddle.Remove(other.gameObject);

            if (damageCoroutines.TryGetValue(other.gameObject, out var coroutine))
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
                damageCoroutines.Remove(other.gameObject);
            }
        }
    }

    private IEnumerator DamageRoutine(GameObject target, IHealth health)
    {
        while (true)
        {
            if (health.CurrentHealth <= 0)
                yield break;

            health.ApplyDamage(tickDamage);
            if (target.TryGetComponent<DamageVisuals>(out var damageVisuals))
            {
                damageVisuals.ShowEffect(DamageVisuals.EffectType.Rot);
            }


            if (target.TryGetComponent<IRoting>(out var rotable))
            {
                if (Random.value <= rotChance)
                {
                    rotable.ApplyRot(
                        rotEffectData.duration,
                        rotEffectData.tickInterval,
                        rotEffectData.tickDamage
                    );
                }
            }

            yield return new WaitForSeconds(tickInterval);
        }
    }
}
