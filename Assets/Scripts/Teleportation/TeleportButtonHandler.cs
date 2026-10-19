using UnityEngine;

public class TeleportButtonHandler : MonoBehaviour
{
    [Tooltip("Куда телепортировать игрока")]
    public Transform teleportTarget;

    [Tooltip("Панель с картой (PanelTeleport)")]
    public GameObject panelTeleport;

    [Tooltip("Стрелка вокруг игрока")]
    public GameObject arrowObject;  // <-- Перетащи Arrow из иерархии!

    public void OnTeleportClick()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && teleportTarget != null)
        {
            player.transform.position = teleportTarget.position;
        }

        if (panelTeleport != null)
        {
            panelTeleport.SetActive(false);
            Time.timeScale = 1f;
        }

        if (arrowObject != null)
        {
            arrowObject.SetActive(true);   // <-- ВКЛЮЧАЕМ стрелку после телепорта!
        }
    }
}
