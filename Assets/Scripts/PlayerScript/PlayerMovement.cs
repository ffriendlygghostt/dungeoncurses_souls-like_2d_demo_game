using UnityEngine;

public class PlayerMovement : MonoBehaviour, IMove
{
    private PlayerStats stats;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float currentStamina;
    private bool isDashing;
    private bool canMove = true;
    private bool canWalk = true;
    private bool canDash = true;

    private float dashCooldown = 0.3f;

    private bool isInvulnerable;
    private float invulnDuration = 0.4f;

    private bool isAttacking = false;
    private Vector2 attackDirection;

    private float speedMultiplier = 1f;
    private float buffSpeedMultiplier = 1f;
    private float dashMultiplier = 1f;
    private bool isFrozen = false;

    private HealingBook healingBook;

    public void SetCanWalk(bool value) => canWalk = value;
    public void SetCanDash(bool value) => canDash = value;

    public void Initialize(PlayerStats s)
    {
        stats = s;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentStamina = stats.maxStamina;
        healingBook = GetComponent<HealingBook>();
    }

    public void HandleMovement()
    {
        if (isDashing) return;

        // --- Ходьба ---
        if (canWalk && canMove)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            Vector2 move = new Vector2(moveX, moveY).normalized * stats.moveSpeed * speedMultiplier * buffSpeedMultiplier;
            rb.velocity = move;

            animator.SetBool("IsRun", move.magnitude > 0);

            // 🔊 Звук шагов
            if (move.magnitude > 0.1f)
            {
                SoundManager.Instance?.PlayFootstep();
            }
            else
            {
                SoundManager.Instance?.StopFootstep();
            }

            if (isAttacking)
            {
                Flip(attackDirection.x < 0);
            }
            else if (move.magnitude > 0)
            {
                Flip(move.x < 0);
            }
            else
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Flip(mouseWorldPos.x < transform.position.x);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("IsRun", false);
            SoundManager.Instance?.StopFootstep(); // остановка при невозможности ходьбы
        }

        // --- Дэш ---
        Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (canDash && canMove && Input.GetKeyDown(KeyCode.Space) && currentStamina >= 20 && inputDir.magnitude > 0)
        {
            healingBook.CancelHeal();
            Dash(inputDir);
        }

        currentStamina = Mathf.Min(currentStamina + stats.staminaRegenRate * Time.deltaTime, stats.maxStamina);
    }

    private void Dash(Vector2 dir)
    {
        isDashing = true;
        isInvulnerable = true;

        // 🔇 Остановить звук шагов
        SoundManager.Instance?.StopFootstep();

        // 🔊 Воспроизвести звук рывка
        SoundManager.Instance?.PlayDash();

        currentStamina -= 20;
        rb.velocity += dir * stats.dashForce * 2f * dashMultiplier;
        animator.SetFloat("DashHorizontal", dir.x);
        animator.SetTrigger("IsDash");

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);

        Invoke(nameof(EndDash), dashCooldown);
        Invoke(nameof(EndInvulnerability), invulnDuration);
    }



    private void EndDash()
    {
        isDashing = false;
        animator.SetFloat("DashHorizontal", 0);
    }

    private void EndInvulnerability()
    {
        isInvulnerable = false;
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    public bool IsInvulnerable() => isInvulnerable;

    public float GetCurrentStamina() => currentStamina;

    public void ReduceStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
    }

    public void SetAttackingState(bool attacking, Vector2 direction)
    {
        isAttacking = attacking;
        if (attacking)
            attackDirection = direction;
    }

    private void Flip(bool faceLeft)
    {
        Vector3 scale = transform.localScale;
        scale.x = faceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public void SetDashMultiplier(float multiplier)
    {
        dashMultiplier = multiplier;
    }

    public void FreezeMovement(bool freeze)
    {
        isFrozen = freeze;
        if (freeze)
        {
            rb.velocity = Vector2.zero;
            canMove = false;
            animator.SetBool("IsRun", false);
            animator.SetBool("IsDash", false);
            animator.speed = 0f;
            canDash = false;
            canWalk = false;
            SoundManager.Instance?.StopFootstep(); // остановка при заморозке
        }
        else
        {
            canMove = true;
            canDash = true;
            canWalk = true;
            animator.speed = 1f;
        }
    }

    public void FreezeMovementNotSetAnim(bool freeze)
    {
        isFrozen = freeze;
        if (freeze)
        {
            rb.velocity = Vector2.zero;
            canMove = false;
            canDash = false;
            canWalk = false;
            SoundManager.Instance?.StopFootstep();
        }
        else
        {
            canMove = true;
            canDash = true;
            canWalk = true;
        }
    }

    public void CanMove(bool move)
    {
        canMove = move;
    }

    public void SetSpeedBuff(float multiplier)
    {
        buffSpeedMultiplier += multiplier;
    }

    public void ResetSpeedBuff()
    {
        buffSpeedMultiplier = 1f;
    }

    public void ResetMovementState()
    {
        isFrozen = false;
        canMove = true;
        canDash = true;
        canWalk = true;
        animator.speed = 1f;
        SoundManager.Instance?.StopFootstep();
    }
}
