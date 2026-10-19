using UnityEditor;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
    public string sortingLayerName = "doorbok";
    public int sortingOrder = 7;
    public bool overrideSpriteSortPoint = false;
    public SpriteSortPoint spriteSortPoint = SpriteSortPoint.Pivot;

    void Start()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = sortingOrder;
            
            if (overrideSpriteSortPoint)
            {
                renderer.spriteSortPoint = spriteSortPoint;
            }
        }
    }
}