using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackSkelet: MonoBehaviour, IEnemyAttack
{
    public int damage = 20;
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

    [Header("Dash")]
    public DashZone dashZone;
    public float dashCooldown = 5f;
    public float dashDelay = 0.3f;

    private bool isDashing = false;
    private bool canDash = true;
    private bool isWaitingToDash = false;

    private LayerMask obstacleLayer;

    private Vector3 dashTarget;       
    private bool hasDealtDamage = false;

    public void Initialize(EnemyStats stats) { }

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        health = GetComponent<IHealth>();
        movement = GetComponent<IEnemyMovement>();

        // Подписка на события входа в зону
        attackZoneLeft.PlayerIsInside += TryAttack;
        attackZoneRight.PlayerIsInside += TryAttack;

        dashZone.OnPlayerEnterZone += TryDash;

        obstacleLayer = LayerMask.GetMask("Wall", "collision");
    }


    void Update()
    {
        currentAttackZone = movement.IsFlipped ? attackZoneLeft : attackZoneRight;

        if (!isDashing && canDash && !isWaitingToDash &&
            dashZone != null && dashZone.IsPlayerInZone &&
            !health.IsDead && !movement.IsFrozenByHurt)
        {
            if (Random.value < 0.015f)
            {
                TryDash();
                return;
            }
        }

        if (!isDashing && canAttack && !isWaitingToAttack && currentAttackZone != null &&
            currentAttackZone.IsPlayerInside && !health.IsDead && !movement.IsFrozenByHurt)
        {
            TryAttack();
        }
    }





    public void TryAttack()
    {
        if (isDashing || isWaitingToDash) return;

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
            StartCoroutine(ZaderjkaVrazvitii(0.4f, 0.3f));
        }

        isWaitingToAttack = false;
        //attackDelay = 0.2f;
    }


    private IEnumerator ZaderjkaVrazvitii(float duration, float duration2)
    {
        yield return new WaitForSeconds(duration);
        movement.StartAttackFreeze(duration2);
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







    public void TryDash()
    {
        if (isDashing || isWaitingToDash) return;

        if (isWaitingToAttack) return;

        if (!canDash || health.IsDead || dashZone == null || !dashZone.IsPlayerInZone) return;

        isWaitingToDash = true;
        GetComponent<Animator>().SetTrigger("IsAttacking2");
        StartCoroutine(ZaderjkaVrazvitii(0f, 0.3f));
    }



    public void OnDashWindupFinished()
    {
        if (dashZone == null || !dashZone.IsPlayerInZone || dashZone.PlayerObject == null)
        {
            isWaitingToDash = false;
            return;
        }



        Vector2 direction = (dashZone.PlayerObject.transform.position - transform.position).normalized;
        float checkDistance = 0.5f; // подгони под своего врага

        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,                   // откуда
            new Vector2(0.4f, 0.4f),              // размер "тела"
            0f,                                   // угол поворота (0)
            direction,                            // направление
            checkDistance,                        // длина
            obstacleLayer                         // слои препятствий
        );

        if (hit.collider != null)
        {
            Debug.Log("Dash blocked by obstacle.");
            isWaitingToDash = false;
            StartCoroutine(DashPenaltyCooldown());
            return;
        }






        // Всё ок — дэш начинается
        dashTarget = dashZone.PlayerObject.transform.position;
        isDashing = true;
        canDash = false;
        hasDealtDamage = false;

        GetComponent<Animator>().SetTrigger("IsDash");
        StartCoroutine(PerformDashMovement());
        isWaitingToDash = false;
    }



    private IEnumerator DashPenaltyCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(3f); 
        canDash = true;
    }



    private IEnumerator PerformDashMovement()
    {
        float dashSpeed = 3f;
        float duration = 0.8f;
        float timer = 0f;

        Vector2 direction = (dashTarget - transform.position).normalized;

        while (timer < duration)
        {
            float step = dashSpeed * Time.deltaTime;

            Vector2 rayOrigin = (Vector2)transform.position + new Vector2(0, 0.17f);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, step, obstacleLayer);

            if (hit.collider != null && hit.collider.gameObject != this.gameObject)
            {
                Vector3 hitPoint = hit.point - (Vector2)(direction * 0.01f); // чуть назад от точки удара
                transform.position = hitPoint;

                EndDash();
                yield break;
            }
            else
            {
                transform.position += (Vector3)(direction * step);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        EndDash();
    }










    private void EndDash()
    {
        isDashing = false;
        movement.StartAttackFreeze(1f);
        
        ZaderjkaVrazvitii(0f, 5f);
        // Откат перед следующим дэшом
        StartCoroutine(DashCooldownRoutine());
    }

    private IEnumerator DashCooldownRoutine()
    {
        yield return new WaitForSeconds(dashCooldown+5f);
        canDash = true;
    }





    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return; 

        if (collision.gameObject.CompareTag("Player"))
        {
            var playerHealth = collision.gameObject.GetComponent<IHealth>();
            if (playerHealth == null)
                playerHealth = collision.gameObject.GetComponentInParent<IHealth>();

            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage / 2);
            }

            var bleedable = collision.gameObject.GetComponent<IBleeding>();
            if (bleedable != null)
            {
                bleedable.ApplyBleed(7.5f, 1f, 2);
            }
        }

        EndDash();
    }










    void OnDestroy()
    {
        // Отписка от событий на всякий случай
        if (attackZoneLeft != null)
            attackZoneLeft.PlayerIsInside -= TryAttack;

        if (attackZoneRight != null)
            attackZoneRight.PlayerIsInside -= TryAttack;

        if (dashZone != null)
            dashZone.OnPlayerEnterZone -= TryDash;
    }

    public void CanAttack(bool canattack)
    {
        canAttack = canattack;
        insideCanAttack = canattack;
    }
}
