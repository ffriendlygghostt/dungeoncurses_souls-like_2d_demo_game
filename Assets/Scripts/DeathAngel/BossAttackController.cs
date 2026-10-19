//using System.Collections;
//using UnityEngine;

//public class BossAttackTeleport : MonoBehaviour, IEnemyAttack
//{
//    public int damage = 45;
//    public float attackDelay = 0.5f;
//    public float cooldown = 3f;
//    public float teleportDistanceBehindPlayer = 1.5f;
//    public float vanishDuration = 1f;

//    private bool canAttack = true;
//    private bool isAttacking = false;

//    private Transform target;
//    private IHealth bossHealth;
//    private IEnemyMovement movement;
//    private Animator animator;

//    void Start()
//    {
//        target = GameObject.FindGameObjectWithTag("Player")?.transform;
//        bossHealth = GetComponent<IHealth>();
//        movement = GetComponent<IEnemyMovement>();
//        animator = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        if (!canAttack || isAttacking || target == null || bossHealth.IsDead || movement.IsFrozenByHurt)
//            return;

//        float distance = Vector2.Distance(transform.position, target.position);
//        if (distance < 10f)
//        {
//            TryAttack();
//        }
//    }

//    public void TryAttack()
//    {
//        if (!canAttack || isAttacking || target == null) return;

//        StartCoroutine(PerformTeleportAttack(playerPos));
//    }

//    private IEnumerator PerformTeleportAttack(Vector2 playerPos)
//    {
//        isAttacking = true;
//        canAttack = false;

//        // Исчезновение
//        animator.SetTrigger("Vanish");
//        SetVisible(false);
//        movement.FreezeMovement(true);

//        yield return new WaitForSeconds(vanishDuration);

//        // Телепорт за игрока
//        Vector2 playerPos = target.position;
//        Vector2 offset = (target.right * -1) * teleportDistanceBehindPlayer;
//        transform.position = playerPos + (Vector3)offset;

//        // Появление
//        animator.SetTrigger("Appear");
//        SetVisible(true);
//        yield return new WaitForSeconds(attackDelay);

//        // Атака
//        animator.SetTrigger("Attack");
//        DealDamageIfInRange();

//        // Задержка после атаки
//        yield return new WaitForSeconds(0.5f);
//        movement.FreezeMovement(false);

//        // КД
//        StartCoroutine(AttackCooldown());

//        isAttacking = false;
//    }

//    private void DealDamageIfInRange()
//    {
//        float dist = Vector2.Distance(transform.position, target.position);
//        if (dist < 2.5f)
//        {
//            target.GetComponent<IHealth>()?.ApplyDamage(damage);
//            Debug.Log("[Boss] Teleport attack hit the player!");
//        }
//    }

//    private IEnumerator AttackCooldown()
//    {
//        yield return new WaitForSeconds(cooldown);
//        canAttack = true;
//    }

//    private void SetVisible(bool visible)
//    {
//        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
//        {
//            renderer.enabled = visible;
//        }
//    }

//    public void Initialize(EnemyStats stats) { }

//    public void CanAttack(bool canattack)
//    {
//        canAttack = canattack;
//    }
//}
