using UnityEngine;

public class MapZoom : MonoBehaviour
{
    public RectTransform mapRect; // Привяжи сюда свой RectTransform карты
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float currentScale = mapRect.localScale.x;
            float newScale = Mathf.Clamp(currentScale + scroll * zoomSpeed, minScale, maxScale);
            mapRect.localScale = new Vector3(newScale, newScale, 1);
        }
    }
}
