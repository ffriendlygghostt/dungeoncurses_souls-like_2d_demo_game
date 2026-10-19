using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationScript : MonoBehaviour
{
    public Transform[] teleportPoints; // ћассив точек телепорта
    public GameObject[] doors; // ћассив дверей
    public int enemyCount = 0; // »значальное количество врагов

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enemyCount == 0)
        {
            // ќпредел€ем индекс двери, в которую вошЄл игрок
            int doorIndex = System.Array.IndexOf(doors, gameObject);

            // ≈сли дверь не найдена в массиве, выходим из метода
            if (doorIndex == -1)
            {
                Debug.LogError("Door not found in the doors array!");
                return;
            }

            // “елепортируем игрока к соответствующей точке телепорта
            if (doorIndex < teleportPoints.Length)
            {
                other.transform.position = teleportPoints[doorIndex].position;
            }
            else
            {
                Debug.LogError("Teleport point not found for door index: " + doorIndex);
            }
        }
    }

    // ћетод дл€ уменьшени€ количества врагов
    public void DecreaseEnemyCount()
    {
        enemyCount--;

        // ѕровер€ем, если количество врагов достигло нул€, разблокируем доступ к двер€м и телепортам
        if (enemyCount == 0)
        {
            UnlockDoorsAndTeleporters();
        }
    }

    // ћетод дл€ разблокировки дверей и телепортов
    private void UnlockDoorsAndTeleporters()
    {
        foreach (GameObject door in doors)
        {
            door.GetComponent<Collider>().enabled = true;
        }
    }
}