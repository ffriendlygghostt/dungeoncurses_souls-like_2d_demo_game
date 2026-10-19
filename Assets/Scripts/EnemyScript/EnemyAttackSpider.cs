using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackSpider : MonoBehaviour, IEnemyAttack
{
    public int damage = 25;
    public float cooldown = 1.5f;
    public float attackDelay = 0.2f;
    //public float defaultAttackDelay = 1f;

    private bool canAttack = true;
    private bool insideCanAttack = false;
    private bool isWaitingToAttack = false;

    protected Transform target;
    private IEnemyMovement movement;
    private IHealth health;

    public AttackZone attackZoneLeft;
    public AttackZone attackZoneRight;

    protected AttackZone currentAttackZone;

    public void Initialize(EnemyStats stats) { }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        health = GetComponent<IHealth>();
        movement = GetComponent<IEnemyMovement>();

        // Подписка на события входа в зону
        attackZoneLeft.PlayerIsInside += TryAttack;
        attackZoneRight.PlayerIsInside += TryAttack;
    }


    void Update()
    {
        currentAttackZone = movement.IsFlipped ? attackZoneLeft : attackZoneRight;

        if (canAttack && !isWaitingToAttack && currentAttackZone != null && currentAttackZone.IsPlayerInside && !health.IsDead && !movement.IsFrozenByHurt)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            //attackDelay = 0.2f;
            TryAttack();
        }
    }




    public void TryAttack()
    {
        if (!canAttack || isWaitingToAttack || health.IsDead) return;


        if (currentAttackZone != null && currentAttackZone.IsPlayerInside)
        {
            StartCoroutine(StartAttackDelay());
            Debug.Log("TryAttack called");
        }
    }

    private IEnumerator StartAttackDelay()
    {
        isWaitingToAttack = true;
        yield return new WaitForSeconds(attackDelay);

        currentAttackZone = movement.IsFlipped ? attackZoneLeft : attackZoneRight;

        if (currentAttackZone != null && currentAttackZone.IsPlayerInside)
        {
            GetComponent<Animator>()?.SetTrigger("IsAttacking");
            health.ParryOn();
            StartCoroutine(AttackCooldown());
            StartCoroutine(ZaderjkaVrazvitii(0.4f));
        }

        isWaitingToAttack = false;
        //attackDelay = 0.2f;
    }


    private IEnumerator ZaderjkaVrazvitii(float duration)
    {
        yield return new WaitForSeconds(duration);
        movement.StartAttackFreeze(0.3f);
    }

    // Вызывается из анимации
    public virtual void DealBiteDamage()
    {
        if (target == null || currentAttackZone == null) return;

        GameObject playerObj = currentAttackZone.PlayerObject;
        if (playerObj != null && currentAttackZone.GetComponent<Collider2D>().OverlapPoint(target.position))
        {
            playerObj.GetComponent<IHealth>()?.ApplyDamage(damage);
        }

        if (currentAttackZone.PlayerObject != null)
        {
            var playerHealth = currentAttackZone.PlayerObject.GetComponent<IHealth>();
            if (playerHealth.IsDead)
            {
                currentAttackZone.NotInside();
            }
 
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(cooldown);
        if (!insideCanAttack)
        {
            canAttack = true;
        }
    }

    void OnDestroy()
    {
        // Отписка от событий на всякий случай
        if (attackZoneLeft != null)
            attackZoneLeft.PlayerIsInside -= TryAttack;

        if (attackZoneRight != null)
            attackZoneRight.PlayerIsInside -= TryAttack;
    }

    public void CanAttack(bool canattack)
    {
        canAttack = canattack;
        insideCanAttack = canattack;
    }
}
