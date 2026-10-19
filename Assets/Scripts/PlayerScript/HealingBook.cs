using UnityEngine;
using TMPro;

public class HealingBook : MonoBehaviour
{
    [Header("Настройки книги")]
    [SerializeField] private int maxUses = 3;
    [SerializeField] private int currentUses = 3;
    public int MaxUses => maxUses;

    [Header("Ссылка на PlayerHealth")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("UI (TextMeshPro)")]
    [SerializeField] private TextMeshProUGUI usesText;

    private bool CanHealing = true;
    private bool isCursed = false; // ❗ Флаг проклятия

    private Animator animator;
    private IMove movement;
    private Rotable rot;

    void Start()
    {
        RefreshUI();
        movement = GetComponent<IMove>();
        animator = GetComponent<Animator>();
        rot = GetComponent<Rotable>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseHeal();
        }
    }

    private void UseHeal()
    {
        if (rot.IsRotting) return;
        if (!CanHealing) return;
        if (currentUses > 0)
        {
            animator.SetTrigger("IsHeal");
            CanHealing = false;
            movement.SetCanWalk(false);
            movement.SetCanDash(true);
        }
    }

    private void Heals()
    {
        currentUses--;

        if (playerHealth != null)
        {
            playerHealth.Heal50Percent();
        }

        if (!isCursed)
            CanHealing = true;

        movement.SetCanWalk(true);
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (usesText != null)
        {
            usesText.text = currentUses.ToString();
        }
    }

    public void AddUses(int amount)
    {
        currentUses += amount;
        if (currentUses > maxUses)
        {
            currentUses = maxUses;
        }
        RefreshUI();
    }

    public void SetMaxUses(int newMax)
    {
        maxUses = newMax;
        if (currentUses > maxUses)
        {
            currentUses = maxUses;
        }
        RefreshUI();
    }

    public void RestoreAllUses()
    {
        currentUses = maxUses;
        RefreshUI();
    }

    public void CantHeal()
    {
        isCursed = true;
        CanHealing = false;
    }

    public void CanHeal()
    {
        if (!isCursed)
            CanHealing = true;
    }

    public void RemoveCurse()
    {
        isCursed = false;
        CanHealing = true;
    }

    public void CancelHeal()
    {
        if (!CanHealing) return;
        CanHealing = true;
        movement.SetCanWalk(true);
    }

    public bool IsHealing => CanHealing;
}
