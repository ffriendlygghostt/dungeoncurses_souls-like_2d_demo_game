using System.Collections;
using UnityEngine;

public class DamageVisuals : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Colors")]
    public Color defaultColor = new Color(1f, 1f, 1f, 1f);
    public Color hitColor = new Color(1f, 0f, 0f, 0.4f);
    public Color burnColor = new Color(1f, 0.5f, 0.2f, 0.8f);
    public Color bleedColor = new Color(0.97f, 0.4f, 0.4f, 0.95f);
    public Color CoolingColor = new Color(0.7f, 0.9f, 1f, 0.6f);
    public Color freezeColor = new Color(0.2f, 0.6f, 1f, 0.4f);
    public Color rotColor = new Color(0.2f, 0.6f, 0.2f, 0.4f);

    private Coroutine flashRoutine;
    private bool isEffectActive = false; // Флаг активности

    public enum EffectType { Hit, Burn, Bleed, Cooling, Freeze, Rot }

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ShowEffect(EffectType type, float duration = 0.3f)
    {
        if (gameObject.name == "Player")
            Debug.Log($"[DamageVisuals] ShowEffect called with type: {type}, duration: {duration}");

        Color colorToFlash = defaultColor;

        switch (type)
        {
            case EffectType.Hit: colorToFlash = hitColor; break;
            case EffectType.Burn: colorToFlash = burnColor; break;
            case EffectType.Bleed: colorToFlash = bleedColor; break;
            case EffectType.Cooling: colorToFlash = CoolingColor; break;
            case EffectType.Freeze: colorToFlash = freezeColor; break;
            case EffectType.Rot: colorToFlash = rotColor; break;
        }

        if (flashRoutine != null)
        {
            if (gameObject.name == "Player")
                Debug.Log("[DamageVisuals] Flash routine already running, stopping it first.");
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(Flash(colorToFlash, duration));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            spriteRenderer.color = new Color(1f, 0f, 0f, 0.5f); // Hit-подсветка
            Debug.Log("[Test] Manual red hit color applied.");
        }
    }


    public void ClearDamageEffects()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        isEffectActive = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColor;
        }
    }

    private IEnumerator Flash(Color color, float duration)
    {
        if (gameObject.name == "Player")
            Debug.Log($"[DamageVisuals] Set color to {color}");

        isEffectActive = true;
        spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = defaultColor;
        isEffectActive = false;
        flashRoutine = null;
    }

    // Публичное свойство для проверки
    public bool IsEffectActive => isEffectActive;
}
