using System.Collections.Generic;
using UnityEngine;

public class SwordBeam : MonoBehaviour
{
    [SerializeField] private float distanceFromPlayer = 0.4f;
    [SerializeField] private float verticalOffset = 0.13f; 


    private Animator animator;
    private Transform playerTransform;
    private PlayerController playerController;
    private PlayerStats playerStats;

    private enum AttackType { None, Fire, Ice }
    private AttackType currentAttackType = AttackType.None;

    private HashSet<IHealth> enemiesInRange = new HashSet<IHealth>();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<PlayerController>();
            playerStats = player.GetComponent<PlayerStats>();
        }

        if (animator == null) Debug.LogError("Animator missing");
        if (playerController == null) Debug.LogError("PlayerController missing");
        if (playerStats == null) Debug.LogError("PlayerStats missing");
    }

    public void Activate(Vector2 direction)
    {
        gameObject.SetActive(true);

        Vector3 offset = direction.normalized * distanceFromPlayer;
        offset += Vector3.up * verticalOffset; // добавляем вертикальное смещение

        transform.position = playerTransform.position + offset;

        bool isFacingRight = direction.x >= 0;
        transform.localScale = isFacingRight ? Vector3.one : new Vector3(-1f, 1f, 1f);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        enemiesInRange.Clear();
        DecideAttackTypeAndAnimate();
    }




    private void DecideAttackTypeAndAnimate()
    {
        var guaranteedEffect = playerStats.TryConsumeGuaranteedEffect();

        switch (guaranteedEffect)
        {
            case PlayerStats.AttackType.Fire:
                currentAttackType = AttackType.Fire;
                animator.SetTrigger("ActivateFire");
                break;
            case PlayerStats.AttackType.Ice:
                currentAttackType = AttackType.Ice;
                animator.SetTrigger("ActivateIce");
                break;
            default:

                float fireRoll = Random.Range(0f, 100f);
                float iceRoll = Random.Range(0f, 100f);

                if (fireRoll <= playerStats.GetFireChance())
                {
                    currentAttackType = AttackType.Fire;
                    animator.SetTrigger("ActivateFire");
                }
                else if (iceRoll <= playerStats.GetIceChance())
                {
                    currentAttackType = AttackType.Ice;
                    animator.SetTrigger("ActivateIce");
                }
                else
                {
                    currentAttackType = AttackType.None;
                    animator.SetTrigger("Activate");
                }
                break;
        }
    }


    public void PerformDamage()
    {
        int baseDamage = playerStats.GetDamage();
        float critChance = playerStats.GetCritChance();
        float critMultiplier = playerStats.GetCritMultiplier();

        bool isCrit = Random.Range(0f, 100f) <= critChance;
        int damage = isCrit ? Mathf.RoundToInt(baseDamage * critMultiplier) : baseDamage;

        if (isCrit) CritDamage();

        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;

            enemy.ApplyDamage(damage);

            switch (currentAttackType)
            {
                case AttackType.Fire:
                    (enemy as Component)?.GetComponent<IIgnitable>()?.Ignite();
                    break;
                case AttackType.Ice:
                    (enemy as Component)?.GetComponent<Freezable>()?.StartFreez();
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IHealth health = other.GetComponent<IHealth>();
            if (health != null) enemiesInRange.Add(health);
        }

        if (other.CompareTag("Torch&Candle"))
        {
            other.GetComponent<IgnitedTorch>()?.Smoking();
            other.GetComponent<ExtinguishedTorch>()?.Smoking();
            other.GetComponent<CandleControllerPuzzle>()?.Smoking();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IHealth health = other.GetComponent<IHealth>();
            if (health != null) enemiesInRange.Remove(health);
        }
    }

    private void CritDamage()
    {
        Debug.Log("CRITICAL HIT!");
        // Визуал/звук
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}