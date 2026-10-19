using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignPivotBttom : MonoBehaviour
{

    void Start()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
            {
                if (renderer.sprite == null) continue;

                float spriteHeight = renderer.sprite.bounds.size.y;
                Vector3 offset = new Vector3(0, spriteHeight / 2f, 0);

                renderer.transform.localPosition = offset;
            }

    }
}
