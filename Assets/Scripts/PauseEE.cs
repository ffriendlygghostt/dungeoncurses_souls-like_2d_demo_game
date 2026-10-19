using UnityEngine;
using UnityEngine.SceneManagement; // Для управления сценами

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    void Start()
    {
        // Скрыть меню паузы при запуске игры
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Если нажата клавиша Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Скрываем меню паузы
        Time.timeScale = 1f; // Возобновляем игру
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); // Показываем меню паузы
        Time.timeScale = 0f; // Останавливаем игровой процесс
        GameIsPaused = true;
    }

    public void ReloadScene() // Метод для перезагрузки сцены
    {
        Time.timeScale = 1f; // Возвращаем нормальное течение времени
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезагружаем текущую сцену
    }

    public void QuitGame() // Метод для выхода из игры
    {
        // Проверяем платформу
#if UNITY_EDITOR
        // В редакторе Unity просто останавливаем игру
        Debug.Log("Игра завершена (это работает только в редакторе)");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // В финальной сборке игры выходим из приложения
        Debug.Log("Выход из игры");
        Application.Quit();
#endif
    }
}
