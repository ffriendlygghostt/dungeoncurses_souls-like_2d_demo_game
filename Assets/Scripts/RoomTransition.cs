using UnityEngine;

public class RoomTransition : MonoBehaviour
{
    public Transform nextRoomSpawnPoint; // Точка спауна в следующей комнате

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Проверяем, что это игрок
        {
            // Перемещаем игрока в следующую комнату
            collision.transform.position = nextRoomSpawnPoint.position;
        }
    }
}
