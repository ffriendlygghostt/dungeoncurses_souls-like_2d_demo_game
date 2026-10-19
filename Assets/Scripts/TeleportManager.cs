using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeleportManager : MonoBehaviour
{
    [System.Serializable]
    public class TeleportEntry
    {
        public Button button;           
        public Transform targetPoint;   
    }

    [Header("Настройки")]
    public List<TeleportEntry> teleportButtons;

    [Header("UI и объекты")]
    public GameObject panelTeleport;
    public GameObject arrowObject;
    public Transform player;

    private void Start()
    {
        if (panelTeleport != null)
            panelTeleport.SetActive(false);

        foreach (var entry in teleportButtons)
        {
            if (entry.button != null && entry.targetPoint != null)
            {
                entry.button.onClick.AddListener(() => TeleportTo(entry.targetPoint));
            }
        }
    }

    public void ToggleTeleportPanel()
    {
        if (panelTeleport == null) return;

        bool isActive = panelTeleport.activeSelf;
        panelTeleport.SetActive(!isActive);

        if (arrowObject != null)
            arrowObject.SetActive(isActive);

        Time.timeScale = isActive ? 1f : 0f;
    }

    public void TeleportTo(Transform target)
    {
        if (player != null && target != null)
        {
            player.position = target.position;
        }

        if (panelTeleport != null)
            panelTeleport.SetActive(false);

        if (arrowObject != null)
            arrowObject.SetActive(true);

        Time.timeScale = 1f;
    }
}
