using UnityEngine;

public class EscapeMenuController : MonoBehaviour
{
    [Header("Объект с панелями вкладок (Menu, Tasks, Map, Inventory)")]
    public GameObject escapeMenuPanel;
    public TabSwitcher tabSwitcher;

    private bool isMenuOpen = false;

    void Start()
    {
        if (escapeMenuPanel != null)
        {
            escapeMenuPanel.SetActive(false);
        } else { Debug.LogWarning("qkaqkqa"); }
        isMenuOpen = false;
    }

    void Update()
    {
        // Открытие/закрытие всего меню
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

        // Открытие/закрытие вкладки карты по "M"
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleSpecificTab(tabSwitcher.ShowMapPanel);
        }

        // Открытие/закрытие вкладки инвентаря по "Tab"
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleSpecificTab(tabSwitcher.ShowInventoryPanel);
        }

        // Открытие/закрытие вкладки заданий по "J"
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleSpecificTab(tabSwitcher.ShowTasksPanel);
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (escapeMenuPanel != null)
        {
            escapeMenuPanel.SetActive(isMenuOpen);
        }

        if (isMenuOpen)
        {
            if (tabSwitcher != null)
            {
                tabSwitcher.ShowMenuPanel(); // По умолчанию всегда открывать "Menu"
            }

            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void ToggleSpecificTab(System.Action showTabAction)
    {
        isMenuOpen = !isMenuOpen;
        if (escapeMenuPanel != null)
        {
            escapeMenuPanel.SetActive(isMenuOpen);
        }

        if (isMenuOpen)
        {
            showTabAction?.Invoke(); // Открываем нужную вкладку
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
