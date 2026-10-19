using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HPB : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TextMeshProUGUI healthChangeText;
    [SerializeField] private PlayerController playerController;

    private Coroutine healthChangeCoroutine;

    void Awake()
    {
        if (playerController == null)
        {
            Transform root = transform.parent;
            if (root != null)
            {
                playerController = root.GetComponentInChildren<PlayerController>(true);
                if (playerController == null)
                    Debug.LogError("Не удалось найти компонент PlayerController среди детей объекта " + root.name);
            }
            else
            {
                Debug.LogError("MainInterface не имеет родителя!");
            }
        }

        Transform barHealthTransform = transform.Find("HealthBar");
        if (barHealthTransform != null)
            healthBar = barHealthTransform.GetComponent<Slider>();

        Transform barStaminaTransform = transform.Find("StaminaBar");
        if (barStaminaTransform != null)
            staminaBar = barStaminaTransform.GetComponent<Slider>();

        if (healthBar == null) Debug.LogWarning("HealthBar не найден!");
        if (staminaBar == null) Debug.LogWarning("StaminaBar не найден!");

        Transform textTransform = transform.Find("HealthBar/HPT");
        if (textTransform != null)
            healthChangeText = textTransform.GetComponent<TextMeshProUGUI>();

        if (healthChangeText == null)
            Debug.LogWarning("healthChangeText is Null");

    }


    void Start()
    {
        if (playerController != null)
        {
            if (healthBar != null)
                healthBar.maxValue = playerController.GetMaxHealth();

            if (staminaBar != null)
                staminaBar.maxValue = playerController.GetMaxStamina();
        }
        else
        {
            Debug.LogWarning("playerController всё ещё null в Start");
        }
    }


    void Update()
    {
        if (playerController != null)
        {
            int health = playerController.GetCurrentHealth();
            healthBar.value = health;

            staminaBar.value = playerController.GetCurrentStamina();

            if (playerController.IsDead())
            {
                healthBar.value = 0;
            }
        }
    }



    public void OnHealthChanged(int changeAmount)
    {
        if (healthChangeCoroutine != null)
        {
            StopCoroutine(healthChangeCoroutine);
        }

        healthChangeCoroutine = StartCoroutine(ShowHealthChange(changeAmount));
    }

    private IEnumerator ShowHealthChange(int changeAmount)
    {
        if (healthChangeText != null)
        {
            healthChangeText.text = (changeAmount > 0 ? "+" : "") + changeAmount.ToString();
            healthChangeText.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            healthChangeText.gameObject.SetActive(false);
        }
    }

    public void ShowHealthDamage(int changeAmount)
    {
        StartCoroutine(ShowHealthChange(changeAmount));
    }

    public void UpdateHealth(int health)
    {
        if (healthBar == null)
        {
            Debug.LogError("healthBar is NULL перед установкой значения!");
        }
        else
        {
            healthBar.value = health;
        }

    }
}
