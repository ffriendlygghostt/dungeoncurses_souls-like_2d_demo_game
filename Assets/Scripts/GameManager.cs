using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Переменная для отслеживания количества врагов
    public static int enemyCount = 0;

    // Метод для уменьшения количества врагов
    public static void DecreaseEnemyCount()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            // Ваш код для открытия дверей или изменения игровой ситуации
        }
    }
}
