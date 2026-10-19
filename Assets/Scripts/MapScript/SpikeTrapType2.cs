using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpikeTrap : MonoBehaviour
{
    [Header("Timings")]
    public float delayBeforeAttack = 1f;
    public float timeOpenAndActive = 1.5f;
    public float stopDamageDelay = 0.3f;
    public float individualDamageCooldown = 1.5f;

    [Header("Damage")]
    public int damage = 10;

    private Animator animator;
    private Collider2D attackCollider;

    private HashSet<Collider2D> entitiesInside = new();
    private Dictionary<Collider2D, float> lastHitTime = new();
    private bool isAttacking = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        attackCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidTarget(other)) return;

        entitiesInside.Add(other);
        lastHitTime.TryAdd(other, -999f); // гарантируем, что есть ключ

        // Важно: даже если уже идёт атака — НЕ запускаем новую
        // но если ловушка не в атаке — запускаем
        if (!isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidTarget(other)) return;

        entitiesInside.Remove(other);
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        yield return new WaitForSeconds(delayBeforeAttack);

        animator.SetBool("IsAttacking", true);

        float attackStartTime = Time.time;
        float attackEndTime = attackStartTime + timeOpenAndActive;

        while (Time.time < attackEndTime)
        {
            ApplyDamageToAll();
            yield return new WaitForSeconds(0.1f); 
        }

        animator.SetBool("IsClosing", true);
        yield return new WaitForSeconds(stopDamageDelay);

        animator.SetBool("IsAttacking", false);
        animator.SetBool("IsClosing", false);

        isAttacking = false;


        if (entitiesInside.Count > 0)
        {
            StartCoroutine(AttackSequence());
        }
    }


    void ApplyDamageToAll()
    {
        float currentTime = Time.time;

        foreach (var entity in entitiesInside)
        {
            if (entity == null) continue;

            if (lastHitTime.TryGetValue(entity, out float lastTime))
            {
                if (currentTime - lastTime < individualDamageCooldown)
                    continue; // Кулдаун ещё не прошёл
            }

            if (entity.TryGetComponent<IHealth>(out var health))
            {
                health.ApplyDamage(damage);
                Debug.Log($">> {entity.name} получил урон от ловушки");
                lastHitTime[entity] = currentTime;
            }
        }
    }

    bool IsValidTarget(Collider2D other)
    {
        return other.CompareTag("Player") || other.CompareTag("Enemy");
    }
}
