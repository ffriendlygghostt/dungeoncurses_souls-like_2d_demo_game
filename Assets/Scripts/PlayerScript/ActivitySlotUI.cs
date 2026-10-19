using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ActivitySlotUI : MonoBehaviour
{
    private ItemData currentItem;
    public Image icon;
    public TextMeshProUGUI cooldownLabel;
    public string keyHint;

    private int index;
    private ActivityManager manager;
    private string keyinteract;
    private Color defaultColor;
    private Image.Type defaultImageType;
    private Sprite defaultSprite;

    private float cooldownTimer = 0f;
    private int currentActivationCount = 0;
    private bool isCoolingDown = false;

    public bool IsOnCooldown => cooldownTimer > 0f;
    public bool CanUseActivity => currentItem != null && !IsOnCooldown;

    public void Init(ActivityManager mgr, int idx)
    {
        manager = mgr;
        index = idx;
        defaultColor = icon.color;
        defaultImageType = icon.type;
        defaultSprite = icon.sprite;
        keyinteract = keyHint;
        cooldownLabel.text = keyinteract;
    }

    public void SetItem(ItemData item)
    {
        currentItem = item;

        if (item != null)
        {
            icon.sprite = item.icon;
            icon.color = Color.white;
            icon.type = Image.Type.Sliced;
        }
        else
        {
            icon.sprite = defaultSprite;
            icon.color = defaultColor;
            icon.type = defaultImageType;
        }

        cooldownTimer = 0f;
        isCoolingDown = false;
        cooldownLabel.text = keyinteract;
        icon.color = Color.white;
        currentActivationCount = 0;
    }

    public void UseActivity(GameObject player)
    {
        if (!CanUseActivity) return;

        currentItem?.effect?.Apply(player, currentItem);

        cooldownTimer = currentItem.cooldown;
        isCoolingDown = true;

        var c = icon.color;
        c.a = 0.2f;
        icon.color = c;
    }


    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownLabel.text = Mathf.Ceil(cooldownTimer).ToString("F0");

            var c = icon.color;
            c.a = 0.2f;
            icon.color = c;
        }
        else if (isCoolingDown)
        {
            cooldownTimer = 0f;
            cooldownLabel.text = keyinteract;
            isCoolingDown = false;
            currentActivationCount = 0;
            StartCoroutine(FadeInIcon());
        }
    }

    private IEnumerator FadeInIcon()
    {
        float duration = 0.5f;
        float timer = 0f;
        Color c = icon.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0.5f, 1f, timer / duration);
            icon.color = c;
            yield return null;
        }

        c.a = 1f;
        icon.color = c;
    }
}
