using UnityEngine;

public class TabSwitcher : MonoBehaviour
{
    [Header("Ссылки на панели")]
    public GameObject panelMenu;       // Панель, которую вы хотите отображать для "Меню"
    public GameObject panelTasks;      // Панель для "Задания"
    public GameObject panelMap;        // Панель для "Карта"
    public GameObject panelInventory;  // Панель для "Инвентарь"

    // Методы для кнопок:

    public void ShowMenuPanel()
    {
        panelMenu.SetActive(true);
        panelTasks.SetActive(false);
        panelMap.SetActive(false);
        panelInventory.SetActive(false);
    }

    public void ShowTasksPanel()
    {
        panelMenu.SetActive(false);
        panelTasks.SetActive(true);
        panelMap.SetActive(false);
        panelInventory.SetActive(false);
    }

    public void ShowMapPanel()
    {
        panelMenu.SetActive(false);
        panelTasks.SetActive(false);
        panelMap.SetActive(true);
        panelInventory.SetActive(false);
    }

    public void ShowInventoryPanel()
    {
        panelMenu.SetActive(false);
        panelTasks.SetActive(false);
        panelMap.SetActive(false);
        panelInventory.SetActive(true);
    }
}
