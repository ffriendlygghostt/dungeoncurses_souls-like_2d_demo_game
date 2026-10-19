using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightOnProximity : MonoBehaviour
{
    public string triggerName = "higlightzone";
    public SpriteRenderer spriteRenderer;
    public Color higlightColor = Color.yellow;
    public float highlightIntensity = 2f;
    public float fadeSpeed = 1.0f;

    private Color originalColor;
    private bool isNear = false;
    
    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        originalColor = spriteRenderer.color;

        Transform trigger = transform.Find(triggerName);
        if (trigger != null)
        {
            Collider2D collider = trigger.GetComponent<Collider2D>();
            if (collider != null && collider.isTrigger == false)
                collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("Highlight trigger not found!");
        }
    }

    void Update()
    {
        Color targetColor = isNear ? higlightColor : originalColor;
        spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * fadeSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isNear = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isNear = false;
    }
}
