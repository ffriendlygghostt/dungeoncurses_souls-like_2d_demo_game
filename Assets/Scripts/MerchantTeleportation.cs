using UnityEngine;
using TMPro; // Для работы с текстом
using UnityEngine.UI; // Для работы с UI

public class MerchantTeleportation : MonoBehaviour
{
    public GameObject teleportConfirmationPanel; // Панель подтверждения
    public TMP_Text confirmationText; // Ссылка на текстовый элемент
    private bool isPlayerInRange = false; // Проверка, находится ли игрок в зоне телепорта

    public Transform merchantTeleportTrigger; // Ссылка на триггер в комнате торговца

    private void Start()
    {
        teleportConfirmationPanel.SetActive(false); // Скрываем панель подтверждения изначально
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowTeleportConfirmation();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true; // Игрок находится в пределах
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false; // Игрок вышел из зоны
            HideTeleportConfirmation();
        }
    }

    private void ShowTeleportConfirmation()
    {
        teleportConfirmationPanel.SetActive(true); // Показываем панель подтверждения
    }

    public void OnYesButtonPressed()
    {
        TeleportToMerchant(); // Логика телепортации
        HideTeleportConfirmation();
    }

    public void OnNoButtonPressed()
    {
        HideTeleportConfirmation();
    }

    private void TeleportToMerchant()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (merchantTeleportTrigger != null)
        {
            player.transform.position = merchantTeleportTrigger.position; // Телепортация игрока к триггеру
        }
    }

    private void HideTeleportConfirmation()
    {
        teleportConfirmationPanel.SetActive(false); // Скрываем панель подтверждения
    }
}
