using System.Collections;
using UnityEngine;

public class EnemyMovementSpider : MonoBehaviour, IEnemyMovement
{
    private Transform target;
    private float walkSpeed;
    private float runSpeed;
    private bool isRunning = false;
    private bool isFrozen = false;
    private bool isFrozenByHurt = false;
    bool isFrozenByAttack = false;

    [Header("Obstacle Avoidance Settings")]
    public float obstacleCheckDistance = 0.5f;
    public LayerMask obstacleLayerMask;

    public float avoidAngleStep = 30f;  // на сколько градусов пробуем поворачивать для обхода
    public int maxAvoidChecks = 6;      // сколько раз проверяем в обе стороны (30°, 60°, 90°...)
    public float stuckTimeBeforeNewTarget = 6f; // сколько секунд паук пытается обойти, потом смена цели
    private Vector2? detourPoint = null;
    private Vector2 lastForward = Vector2.right; // последнее направление движения

    private float stuckTimer = 0f;
    private float detourStuckTimer = 0f;


    [Header("Raycast Settings")]
    public float raycastYOffset = 0.5f;


    private float flipThreshold = 0.1f; // Минимальное значение, при котором считаем направление значимым
    private float lastDirectionX = 0f;


    public bool IsFrozen => isFrozen;

    private bool IsMovementFrozen => isFrozen || isFrozenByHurt || isFrozenByAttack;


    private Vector2 patrolCenter;
    private Bounds patrolBounds;
    private Vector2 currentPatrolTarget;
    private SpriteRenderer spriteRenderer;
    private bool isWaiting = false;

    private Animator animator;
    private IHealth playerHealth;

    private IHealth spyderHealth;
    private IEnemyAttack attack;
    public Rigidbody2D rb;

    [Header("Patrol Settings")]
    public Collider2D patrolZone;
    public float waitTimeMin = 2f;
    public float waitTimeMax = 7f;

    [Header("Animation Settings")]
    public float runAnimationSpeed = 1.3f;

    private float speedMultiplier = 1f;




    public bool IsFlipped { get; private set; }



    public void Initialize(EnemyStats stats)
    {
        walkSpeed = stats.walkSpeed;
        runSpeed = stats.runSpeed;

        animator = GetComponent<Animator>();

        if (patrolZone == null)
            patrolZone = FindZoneByName("PatrolZone");

        if (patrolZone != null)
        {
            patrolCenter = patrolZone.bounds.center;
            patrolBounds = patrolZone.bounds;
        }
        else
        {
            patrolCenter = transform.position;
            Debug.LogWarning($"[Spider] PatrolZone not found on {gameObject.name}");
        }

        spyderHealth = GetComponent<IHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        attack = GetComponent<IEnemyAttack>();

        GenerateNewPatrolPoint();
    }


    public void SetTarget(Transform target)
    {
        if (isFrozen) return;
        this.target = target;

        if (target != null)
            playerHealth = target.GetComponent<IHealth>();
    }

    public void SetRunning(bool running)
    {
        if (isFrozen) return;
        isRunning = running;
        if (animator != null)
            animator.speed = running ? runAnimationSpeed : 1f;
    }


    public void Move()
    {
        if (spyderHealth.IsDead) return;
        if (isFrozenByHurt) return;
        if (isFrozen) return;

        if (target != null)
        {
            Vector2 targetPosition = detourPoint ?? (Vector2)target.position;
            Vector2 toTarget = targetPosition - (Vector2)transform.position;


            if (detourPoint.HasValue)
            {
                detourStuckTimer += Time.deltaTime;

                if (detourStuckTimer > 2f) // если слишком долго идёт к точке
                {
                    detourPoint = null;
                    detourStuckTimer = 0f;
                }
            }
            else
            {
                detourStuckTimer = 0f;
            }




            if (toTarget.magnitude < 0.1f)
            {
                SetAnimatorWalk(false);
                // Если достигли точки обхода, сбрасываем её, чтобы идти дальше к основной цели
                if (detourPoint.HasValue)
                    detourPoint = null;
                return;
            }

            Vector2 forward = toTarget.normalized;

            lastForward = forward;

            if (!TryFindAvoidDirection(forward, out Vector2 moveDirection))
            {
                stuckTimer += Time.deltaTime;

                if (stuckTimer > 1.5f && !detourPoint.HasValue)
                {
                    // Паническая точка — уходим в случайную сторону
                    Vector2 randomDir = Random.insideUnitCircle.normalized;
                    Vector2 panicPoint = (Vector2)transform.position + randomDir * 1.5f;
                    detourPoint = panicPoint;

                    moveDirection = (panicPoint - (Vector2)transform.position).normalized;

                    // Не возвращаемся — продолжаем двигаться
                }
                else
                {
                    if (stuckTimer >= stuckTimeBeforeNewTarget)
                    {
                        stuckTimer = 0f;
                        detourPoint = null;
                        SetTarget(null);
                    }

                    SetAnimatorWalk(false);
                    return;
                }
            }


            float distanceToTarget = toTarget.magnitude;
            float speed = (isRunning ? runSpeed : walkSpeed) * speedMultiplier;

            Vector2 moveStep = moveDirection * speed * Time.deltaTime;
            if (moveStep.magnitude > distanceToTarget)
                moveStep = targetPosition - (Vector2)transform.position;

            Vector3 newPos = (Vector2)transform.position + moveStep;
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

            FlipSprite(moveDirection);
            SetAnimatorWalk(true);
        }
        else
        {
            Patrol();
        }
    }





    private void Patrol()
    {
        if (spyderHealth.IsDead || isFrozen || isFrozenByHurt || isWaiting)
            return;

        Vector2 direction = currentPatrolTarget - (Vector2)transform.position;
        float distance = direction.magnitude;

        if (distance < 0.5f)
        {
            SetAnimatorWalk(false);
            StartCoroutine(WaitAtPoint());
            return;
        }
        Vector2 normalizedDir = direction.normalized;
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * raycastYOffset;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, normalizedDir, distance, obstacleLayerMask);
        if (hit.collider != null)
        {
            GenerateNewPatrolPoint();
            return;
        }
        Vector3 newPos = Vector2.MoveTowards(transform.position, currentPatrolTarget, walkSpeed * speedMultiplier * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        FlipSprite(direction);
        SetAnimatorWalk(true);
    }




    private IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        SetAnimatorWalk(false);

        float waitTime = Random.Range(waitTimeMin, waitTimeMax);


        yield return new WaitForSeconds(waitTime);

        GenerateNewPatrolPoint();
        isWaiting = false;
    }


    private void GenerateNewPatrolPoint()
    {
        if (patrolZone != null)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 randomPoint = new Vector2(
                    Random.Range(patrolBounds.min.x, patrolBounds.max.x),
                    Random.Range(patrolBounds.min.y, patrolBounds.max.y)
                );

                if (patrolZone.OverlapPoint(randomPoint))
                {
                    currentPatrolTarget = randomPoint;
                    return;
                }
            }

            currentPatrolTarget = patrolZone.bounds.center;
        }
        else
        {
            currentPatrolTarget = patrolCenter;
        }
    }


    private void SetAnimatorWalk(bool isWalking)
    {
        if (animator != null && !spyderHealth.IsDead)
            animator.SetBool("IsWalking", isWalking);
    }

    private Collider2D FindZoneByName(string name)
    {
        foreach (var col in GetComponentsInChildren<Collider2D>(true))
        {
            if (col.gameObject.name == name)
                return col;
        }

        return null;
    }

    private void FlipSprite(Vector2 direction)
    {
        if (spriteRenderer == null) return;
        if (Mathf.Abs(direction.x) < 0.05f) return;

        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
            IsFlipped = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
            IsFlipped = false;
        }
    }


    public void FreezeMovement(bool freeze)
    {
        isFrozen = freeze;
        ApplyFreezeState();
    }


    public void StartHurtFreeze(float duration)
    {
        if (isFrozen) return;

        if (!isFrozenByHurt)
        {
            StartCoroutine(HurtFreezeCoroutine(duration));
        }
    }

    private IEnumerator HurtFreezeCoroutine(float duration)
    {
        isFrozenByHurt = true;
        ApplyFreezeState();

        yield return new WaitForSeconds(duration);
        if (!spyderHealth.IsDead) 
        { 
            isFrozenByHurt = false;
            ApplyFreezeState();
        }
    }

    public bool IsFrozenByHurt => isFrozenByHurt;

    private void ApplyFreezeState()
    {
        bool frozen = IsMovementFrozen;


        if (!spyderHealth.IsDead)
            SetAnimatorWalk(false);

        if (rb != null)
            rb.constraints = frozen ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;

        attack?.CanAttack(!frozen);
        SetSpeedMultiplier(frozen ? 0f : 1f);
    }


    public void StartAttackFreeze(float duration)
    {
        if (!isFrozenByAttack && !isFrozenByHurt)
        {
            StartCoroutine(AttackFreezeCoroutine(duration));
        }
    }

    private IEnumerator AttackFreezeCoroutine(float duration)
    {

        isFrozenByAttack = true;
        ApplyFreezeState();

        yield return new WaitForSeconds(duration);
        if (!spyderHealth.IsDead)
        {
            isFrozenByAttack = false;
            ApplyFreezeState();
        }
    }


    private bool TryFindAvoidDirection(Vector2 forward, out Vector2 avoidDir)
    {
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * raycastYOffset;
        Vector2 perpendicular = new Vector2(-forward.y, forward.x).normalized * 0.3f;

        Vector2 originCenter = rayOrigin;
        Vector2 originLeft = originCenter + perpendicular;
        Vector2 originRight = originCenter - perpendicular;

        bool hitCenter = Physics2D.Raycast(originCenter, forward, obstacleCheckDistance, obstacleLayerMask);
        bool hitLeft = Physics2D.Raycast(originLeft, forward, obstacleCheckDistance, obstacleLayerMask);
        bool hitRight = Physics2D.Raycast(originRight, forward, obstacleCheckDistance, obstacleLayerMask);

        if (!hitCenter && !hitLeft && !hitRight)
        {
            avoidDir = forward;
            return true;
        }

        for (int i = 1; i <= maxAvoidChecks; i++)
        {
            float angleRight = avoidAngleStep * i;
            float angleLeft = -avoidAngleStep * i;

            Vector2 dirRight = RotateVector(forward, angleRight);
            if (!Physics2D.Raycast(rayOrigin, dirRight, obstacleCheckDistance, obstacleLayerMask))
            {
                Vector2 newPoint = (Vector2)transform.position + dirRight * obstacleCheckDistance * 2f;

                if (Vector2.Distance(transform.position, newPoint) < 0.5f)
                    continue;

                if (detourPoint.HasValue && Vector2.Distance(detourPoint.Value, newPoint) < 0.4f)
                    continue;

                detourPoint = newPoint;
                avoidDir = dirRight;
                return true;
            }

            Vector2 dirLeft = RotateVector(forward, angleLeft);
            if (!Physics2D.Raycast(rayOrigin, dirLeft, obstacleCheckDistance, obstacleLayerMask))
            {
                Vector2 newPoint = (Vector2)transform.position + dirLeft * obstacleCheckDistance * 2f;

                if (Vector2.Distance(transform.position, newPoint) < 0.5f)
                    continue;

                if (detourPoint.HasValue && Vector2.Distance(detourPoint.Value, newPoint) < 0.4f)
                    continue;

                detourPoint = newPoint;
                avoidDir = dirLeft;
                return true;
            }
        }

        avoidDir = Vector2.zero;
        return false;
    }





    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos).normalized;
    }




    public void SetSpeedMultiplier(float value)
    {
        speedMultiplier = value;
    }





    private void OnDrawGizmos()
    {
        Vector3 originCenter = transform.position + Vector3.up * raycastYOffset;

        Vector3 forward3D = lastForward.normalized;
        Vector3 perpendicular3D = new Vector3(-lastForward.y, lastForward.x, 0).normalized * 0.3f;

        Vector3 originLeft = originCenter + perpendicular3D;
        Vector3 originRight = originCenter - perpendicular3D;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(originLeft, originLeft + forward3D * obstacleCheckDistance);
        Gizmos.DrawLine(originCenter, originCenter + forward3D * obstacleCheckDistance);
        Gizmos.DrawLine(originRight, originRight + forward3D * obstacleCheckDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(originCenter, 0.05f);

        if (detourPoint.HasValue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(detourPoint.Value, 0.1f);
            Gizmos.DrawLine(transform.position, detourPoint.Value);
        }
    }




}
