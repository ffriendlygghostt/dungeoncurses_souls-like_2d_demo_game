using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    // Метод, который будет вызываться при нажатии кнопки
    public void ReloadSceneOnButtonPress()
    {
        // Получаем индекс текущей активной сцены
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // Перезагружаем сцену по ее индексу
        SceneManager.LoadScene(activeSceneIndex);
    }
}