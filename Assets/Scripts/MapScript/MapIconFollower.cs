using UnityEngine;

public class MapIconFollower : MonoBehaviour
{
    public RectTransform iconOnMap;
    public Transform player;
    public BoxCollider2D worldBounds;
    public RectTransform mapUIRect;
    public BoxCollider2D mapBounds;

    void Start()
    {
        UpdateIconPosition(); 
    }

    void Update()
    {
        UpdateIconPosition();
    }

    void UpdateIconPosition()
    {
        Vector2 playerPos = player.position;

        Vector2 worldMin = worldBounds.bounds.min;
        Vector2 worldMax = worldBounds.bounds.max;

        Vector2 mapMin = mapBounds.bounds.min;
        Vector2 mapMax = mapBounds.bounds.max;

        float normX = Mathf.InverseLerp(worldMin.x, worldMax.x, playerPos.x);
        float normY = Mathf.InverseLerp(worldMin.y, worldMax.y, playerPos.y);

        float uiWorldX = Mathf.Lerp(mapMin.x, mapMax.x, normX);
        float uiWorldY = Mathf.Lerp(mapMin.y, mapMax.y, normY);
        Vector3 uiWorldPos = new Vector3(uiWorldX, uiWorldY, 0);

        Vector2 localUIPos = mapUIRect.InverseTransformPoint(uiWorldPos);
        iconOnMap.anchoredPosition = localUIPos;
    }
}
