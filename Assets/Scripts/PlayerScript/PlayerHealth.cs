using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class PlayerHealth : MonoBehaviour, IHealth
{
    // Статический счётчик всех смертей (по простому).
    // При каждой смерти увеличиваем, а в InventoryManager добавляем проклятие.
    private static int deathCount = 0;

    private PlayerStats stats;
    private Animator animator;
    private int currentHealth;
    private IMove move;
    private PlayerAttack attack;
    private HealingBook bookheal;
    private Rigidbody2D rb;
    [SerializeField] private Transform defaultSpawnPoint;
    private DamageVisuals damageVisuals;


    public void Initialize(PlayerStats s)
    {
        stats = s;
        animator = GetComponent<Animator>();
        currentHealth = stats.maxHealth;
        FindObjectOfType<HPB>().UpdateHealth(currentHealth);
        move = GetComponent<IMove>();
        attack = GetComponent<PlayerAttack>();
        bookheal = GetComponent<HealingBook>();
        rb = GetComponent<Rigidbody2D>();
        damageVisuals = GetComponent<DamageVisuals>();

    }

    public void ApplyDamage(int amount)
    {
        ApplyDamage(amount, Color.white);
    }

    public void ApplyDamage(int amount, Color color)
    {
        if (currentHealth <= 0) return;

        var movement = GetComponent<PlayerMovement>();
        if (movement != null && movement.IsInvulnerable())
            return;

        currentHealth -= amount;
        FindObjectOfType<HPB>().OnHealthChanged(-amount);

        if (damageVisuals != null && !damageVisuals.IsEffectActive)
        {
            damageVisuals.ShowEffect(DamageVisuals.EffectType.Hit);
        }

        if (currentHealth <= 0)
            Die();
    }



    private void ApplyCurse()
    {
        var curseManager = FindObjectOfType<CurseManager>();
        if (curseManager != null)
        {
            curseManager.ApplyRandomCurse(gameObject); // Передаём игрока
        }
    }



    public void Heal50Percent()
    {
        int healAmount = stats.maxHealth / 2;
        currentHealth += healAmount;

        if (currentHealth > stats.maxHealth)
        {
            currentHealth = stats.maxHealth;
        }

        FindObjectOfType<HPB>().OnHealthChanged(healAmount);

        // 🔊 звук лечения
        SoundManager.Instance?.PlayHeal();
    }


    private void Die()
    {
        move.FreezeMovementNotSetAnim(true);
        rb.velocity = Vector2.zero;
        attack.CanAttack(false);
        animator.SetTrigger("IsDead");
        bookheal.CantHeal();
        rb.mass = 3000;





        GetComponent<Bleedable>()?.StopBleeding();
        GetComponent<Burnable>()?.Extinguish();
        GetComponent<Freezable>()?.RemoveFreezeP();
        GetComponent<Freezable>()?.RemoveChillP();
        GetComponent<Rotable>()?.StopRot();
        GetComponent<DamageVisuals>()?.ClearDamageEffects();
        StartCoroutine(Restart(5f));


    }


    private IEnumerator Restart(float delay)
    {
        yield return new WaitForSeconds(delay);
        RestartLevel();
    }

    private void RestartLevel()
    {
        if (PlayerRespawnManager.HasRested)
        {
            transform.position = PlayerRespawnManager.LastRestPosition;
        }
        else
        {
            // 👇 Новый код — не перезагружаем сцену, а телепортируем на начальную точку
            transform.position = defaultSpawnPoint.position;
        }

        Initialize(stats);
        move.FreezeMovementNotSetAnim(false);
        attack.CanAttack(true);
        rb.mass = 1f;


        var renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            Color c = renderer.color;
            renderer.color = new Color(c.r, c.g, c.b, 1f);
        }

        GetComponent<Bleedable>()?.StopBleeding();
        GetComponent<Burnable>()?.Extinguish();
        GetComponent<Freezable>()?.RemoveFreezeP();
        GetComponent<Freezable>()?.RemoveChillP();
        GetComponent<Rotable>()?.StopRot();
        GetComponent<DamageVisuals>()?.ClearDamageEffects();
        GetComponent<HealingBook>()?.RestoreAllUses();

        animator.Rebind();
        animator.Update(0f);

        ApplyCurse();
    }




    public void Resurrection()
    {

    }


    public void OnFadeInComplete()
    {
        StartCoroutine(FadeOut(1.2f));
    }


    private IEnumerator FadeOut(float duration)
    {
        var renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer == null)
            yield break;

        Color originalColor = renderer.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Чтобы точно обнулить
        renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }


    public int GetCurrentHealth() => currentHealth;

    public void ParryOn()
    {
        throw new System.NotImplementedException();
    }


    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > stats.maxHealth)
            currentHealth = stats.maxHealth;

        FindObjectOfType<HPB>().OnHealthChanged(amount);

        // 🔊 звук лечения
        SoundManager.Instance?.PlayHeal();
    }




    public bool IsDead => currentHealth <= 0;
    public int CurrentHealth => currentHealth;
}
