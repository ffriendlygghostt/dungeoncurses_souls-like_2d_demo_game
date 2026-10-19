using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerAttack), typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerMovement movement;
    private PlayerAttack combat;
    private PlayerHealth health;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerAttack>();
        health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        movement.Initialize(stats);
        combat.Initialize(stats);
        health.Initialize(stats);
    }

    private void Update()
    {
        movement.HandleMovement();
        combat.HandleCombat();
    }

    public void TakeDamage(int amount)
    {
        health.ApplyDamage(amount);
    }

    public int GetCurrentHealth() => health.GetCurrentHealth();
    public float GetCurrentStamina() => movement.GetCurrentStamina();
    public int GetMaxHealth() => stats.maxHealth;
    public float GetMaxStamina() => stats.maxStamina;
    public bool IsDead() => health.IsDead;
    public int GetDamage() => stats.damageAmount;
    public float GetCritChance() => stats.critChance;
    public float GetCritMultiplier() => stats.critMultiplier;
}
