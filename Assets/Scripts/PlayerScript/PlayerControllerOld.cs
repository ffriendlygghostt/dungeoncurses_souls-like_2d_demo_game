//using UnityEngine;
//using System.Collections;
//using System.Diagnostics;
//using UnityEngine.SceneManagement;


//public class PlayerControllerOld : MonoBehaviour
//{
//    [SerializeField] public float moveSpeed = 2f;
//    [SerializeField] public float dashForce = 1f;
//    [SerializeField] private float attackImpulse = 0.15f;
//    [SerializeField] public int maxHealth = 100;
//    [SerializeField] public int maxStamina = 100;
//    [SerializeField] private int currentHealth;
//    [SerializeField] private float currentStamina;
//    [SerializeField] private SwordBeam swordBeam;
//    public float staminaRegenRate = 10f;
//    private Animator animator;
//    private Rigidbody2D rb;
//    private bool isAttacking = false;
//    private bool isDashing = false;
//    private Vector2 cursorDirection;
//    public int damageAmount = 25;
//    public float attackRange = 1f;
//    public LayerMask enemyLayer;
//    internal bool canMove { get; set; } = true;
//    internal bool canAttack { get; set; } = true;
//    internal bool canDash { get; set; } = true;
//    private string[] attackAnimations = { "IsAttack", "IsAttack2", "IsAttack3" };
//    private bool isEvasionActive = false;
//    private float evasionChance = 0f;

//    private float dashStartTime; // Время начала дэша
//    private float dashEndTime; // Время окончания дэша
//    private float dashInvulnerabilityDuration = 0.3f; // основное время игнорирования урона
//    private float extraInvulnerabilityTime = 0.8f; // дополнительное время игнорирования урона
//    private Stopwatch dashStopwatch = new Stopwatch();
//    private Stopwatch invulnerabilityStopwatch = new Stopwatch();

//    private float dashHorizontal;

//    private void Start()
//    {
//        animator = GetComponent<Animator>();
//        rb = GetComponent<Rigidbody2D>();
//        currentHealth = maxHealth;
//        currentStamina = maxStamina;
//        UnityEngine.Debug.Log("хп игрока - " + currentHealth);
//    }

//    private void Update()
//    {
//        if (!isAttacking && !isDashing && canMove)
//        {
//            float moveHorizontal = Input.GetAxisRaw("Horizontal");
//            float moveVertical = Input.GetAxisRaw("Vertical");
//            Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized * moveSpeed;
//            rb.velocity = movement;

//            if (movement.magnitude > 0)
//            {
//                animator.SetBool("IsRun", true);
//                if (moveHorizontal > 0)
//                {
//                    FlipCharacter(false);
//                }
//                else if (moveHorizontal < 0)
//                {
//                    FlipCharacter(true);
//                }
//            }
//            else
//            {
//                animator.SetBool("IsRun", false);
//            }
//        }

//        if (Input.GetMouseButtonDown(0) && !isAttacking && !isDashing && canAttack && currentStamina >= 30)
//        {
//            RandomAttack();
//            currentStamina -= 30;  // Уменьшение стамины при атаке
//            StartCoroutine(AttackCooldown()); // Начинаем кулдаун после атаки
//        }

//        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && !isDashing && canDash && currentStamina >= 20 && rb.velocity.magnitude > 0)
//        {
//            Dash();
//            currentStamina -= 20;
//        }

//        currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
//        cursorDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
//        dashHorizontal = rb.velocity.normalized.x;
//    }

//    private void FixedUpdate()
//    {
//        if (!isAttacking && !isDashing && canMove)
//        {
//            if (cursorDirection.x > 0 && transform.localScale.x < 0)
//            {
//                FlipCharacter(false);
//            }
//            else if (cursorDirection.x < 0 && transform.localScale.x > 0)
//            {
//                FlipCharacter(true);
//            }
//        }
//    }

//    private void RandomAttack()
//    {
//        string randomAttackAnimation = attackAnimations[Random.Range(0, attackAnimations.Length)];
//        Vector2 attackDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
//        rb.velocity = Vector2.zero; // Остановить игрока перед атакой
//        FlipCharacter(attackDirection.x < 0); // Повернуть игрока в сторону атаки
//        rb.AddForce(attackDirection * attackImpulse, ForceMode2D.Impulse);
//        animator.SetTrigger(randomAttackAnimation);
//        swordBeam.Activate(attackDirection);
//        isAttacking = true;
//        Invoke("ResetAttackState", 0.3f);
//    }

//    private void ResetAttackState()
//    {
//        isAttacking = false;
//        swordBeam.Deactivate();
//    }

//    private IEnumerator AttackCooldown()
//    {
//        canAttack = false; // Запрещаем атаку
//        yield return new WaitForSeconds(0.8f); // Ждем 0.8 секунды
//        canAttack = true; // Разрешаем атаку
//    }

//    private void Dash()
//    {
//        if (rb.velocity.magnitude > 0)
//        {
//            Vector2 dashDirection = rb.velocity.normalized;
//            rb.velocity += dashDirection * dashForce * 2; // Увеличиваем импульс
//            animator.SetFloat("DashHorizontal", dashDirection.x); // Устанавливаем направление дэша для анимации
//            animator.SetTrigger("IsDash");
//            isDashing = true;
//            canAttack = false; // Запрещаем атаку во время дэша

//            dashStopwatch.Restart(); // Начинаем измерение времени дэша
//            invulnerabilityStopwatch.Restart(); // Начинаем измерение времени неуязвимости

//            Invoke("ResetDashState", 0.3f); // Увеличиваем время анимации для более плавного дэша
//            Invoke("EndInvulnerability", 0.8f); // Продлеваем неуязвимость
//        }
//    }

//    private bool EvadeDash()
//    {
//        float evasionChance = 0.5f;
//        float randomValue = Random.value;

//        if (randomValue <= evasionChance)
//        {
//            return true;
//        }
//        else
//        {
//            return false;
//        }
//    }

//    private void ResetDashState()
//    {
//        isDashing = false;
//        canAttack = true; // Разрешаем атаку после дэша
//        animator.SetFloat("DashHorizontal", 0); // Сбрасываем значение анимации дэша

//        dashStopwatch.Stop();
//    }

//    private void EndInvulnerability()
//    {
//        invulnerabilityStopwatch.Stop();
//    }

//    public void TakeDamage(int damageAmount)
//    {
//        float randomValue = Random.value;
//        bool evaded = false;

//        if (isEvasionActive && randomValue <= evasionChance)
//        {
//            evaded = true;
//        }
//        else if (EvadeDash() || isDashing)
//        {
//            evaded = true;
//        }

//        if (!evaded)
//        {
//            currentHealth -= damageAmount;
//            FindObjectOfType<HPB>().OnHealthChanged(-damageAmount);
//            if (currentHealth <= 0)
//            {
//                Die();
//            }
//        }
//    }

//    private void FlipCharacter(bool isFacingLeft)
//    {
//        Vector3 scale = transform.localScale;
//        if (isFacingLeft)
//        {
//            scale.x = Mathf.Abs(scale.x) * -1f;
//        }
//        else
//        {
//            scale.x = Mathf.Abs(scale.x);
//        }
//        transform.localScale = scale;
//    }

//    private void Die()
//    {
//        currentHealth = 0;
//        FindObjectOfType<HPB>().UpdateHealth(currentHealth);
//        animator.SetTrigger("IsDead");
//        Invoke("RestartGame", 2f);
//    }


//    private void RestartGame()
//    {

//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//    }

//    public void SetCanMove(bool moveAllowed)
//    {
//        canMove = moveAllowed;
//    }

//    public void SetHealth(int health)
//    {
//        currentHealth = health;
//    }

//    public void SetEvasionActive(bool active, float chance)
//    {
//        isEvasionActive = active;
//        evasionChance = chance;
//    }

//    public int GetCurrentHealth()
//    {
//        return currentHealth;
//    }

//    public float GetCurrentStamina()
//    {
//        return currentStamina;
//    }

//    public int GetMaxHealth()
//    {
//        return maxHealth;
//    }

//    public float GetMaxStamina()
//    {
//        return maxStamina;
//    }

//    public bool IsDead()
//    {
//        return GetCurrentHealth() <= 0;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, attackRange);
//    }
//}