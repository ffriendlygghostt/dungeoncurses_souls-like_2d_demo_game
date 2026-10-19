using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public Transform player;                      // Игрок
    public GameObject miniMapPanel;               // Объект миникарты
    public BoxCollider2D worldBounds;             // Границы мира (BoxCollider2D на сцене)
    public RectTransform mapImageRect;            // Карта, которая будет двигаться (UI)
    public RectTransform maskRect;                // Маска, по центру которой игрок

    private bool isMiniMapActive = false;

    void Start()
    {
        if (miniMapPanel != null)
            miniMapPanel.SetActive(isMiniMapActive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            isMiniMapActive = !isMiniMapActive;
            if (miniMapPanel != null)
                miniMapPanel.SetActive(isMiniMapActive);
        }

        if (isMiniMapActive)
        {
            UpdateMapPosition();
        }
    }

    void UpdateMapPosition()
    {
        if (player == null || worldBounds == null || mapImageRect == null || maskRect == null)
            return;

        // Получаем границы мира
        Vector2 worldMin = worldBounds.bounds.min;
        Vector2 worldMax = worldBounds.bounds.max;
        Vector2 playerPos = player.position;

        // Нормализуем позицию игрока относительно границ мира
        float normX = Mathf.InverseLerp(worldMin.x, worldMax.x, playerPos.x);
        float normY = Mathf.InverseLerp(worldMin.y, worldMax.y, playerPos.y);

        // Размер карты в UI
        float mapWidth = mapImageRect.rect.width;
        float mapHeight = mapImageRect.rect.height;

        // Вычисляем новую позицию карты: сдвигаем карту так, чтобы игрок оказался в центре маски
        float posX = Mathf.Lerp(-mapWidth / 2f, mapWidth / 2f, 1 - normX);
        float posY = Mathf.Lerp(-mapHeight / 2f, mapHeight / 2f, 1 - normY);

        mapImageRect.anchoredPosition = new Vector2(posX, posY);
    }
}
