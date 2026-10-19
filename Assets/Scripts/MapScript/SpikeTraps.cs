using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [Header("Timings")]
    public float timeBeforeOpen = 2.5f;
    public float timeOpenAndActive = 1.5f;
    public float damageActivationDelay = 0.4f;
    public float stopDamageDelay = 0.3f;
    public float damageCooldown = 1.5f;

    [Header("Damage")]
    public int damage = 10;

    private Animator animator;
    private Collider2D attackCollider;
    private bool canDamage = false;

    private readonly Dictionary<Collider2D, Coroutine> activeCoroutines = new();

    void Start()
    {
        animator = GetComponent<Animator>();
        attackCollider = GetComponent<Collider2D>();
        StartCoroutine(Cycle());
    }

    IEnumerator Cycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBeforeOpen);

            animator.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(damageActivationDelay);

            canDamage = true;

            yield return new WaitForSeconds(timeOpenAndActive);

            animator.SetBool("IsClosing", true);
            yield return new WaitForSeconds(stopDamageDelay);

            canDamage = false;

            yield return new WaitForSeconds(0.2f);
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsClosing", false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IHealth>(out var health))
        {
            if (!activeCoroutines.ContainsKey(other))
            {
                Debug.Log($">> {other.name} вошёл в зону ловушки");
                Coroutine routine = StartCoroutine(DamageRoutine(other, health));
                activeCoroutines.Add(other, routine);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (activeCoroutines.TryGetValue(other, out var routine))
        {
            Debug.Log($"<< {other.name} покинул зону ловушки");
            StopCoroutine(routine);
            activeCoroutines.Remove(other);
        }
    }

    IEnumerator DamageRoutine(Collider2D target, IHealth health)
    {
        while (true)
        {
            if (canDamage)
            {
                Debug.Log($">> {target.name} получил урон от шипов: {damage}");
                health.ApplyDamage(damage);
                yield return new WaitForSeconds(damageCooldown);
            }
            else
            {
                yield return null;
            }
        }
    }
}
