using UnityEngine;

public class RoomTransitionSeller : MonoBehaviour
{
    public Transform nextRoomSpawnPoint; // Точка спауна в следующей комнате
    private bool playerInTrigger = false; // Переменная для отслеживания состояния игрока

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E)) // Проверяем, что игрок в триггере и нажал клавишу E
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = nextRoomSpawnPoint.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Проверяем, что это игрок
        {
            playerInTrigger = true; // Устанавливаем флаг, что игрок в триггере
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Проверяем, что это игрок
        {
            playerInTrigger = false; // Сбрасываем флаг, что игрок вышел из триггера
        }
    }
}
