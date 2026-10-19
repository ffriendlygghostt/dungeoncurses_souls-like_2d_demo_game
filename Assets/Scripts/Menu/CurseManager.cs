using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurseManager : MonoBehaviour
{
    [Header("—писок возможных прокл€тий")]
    public List<ItemData> availableCurses;

    [Header("UI слот дл€ отображени€ иконки прокл€ти€")]
    public Image curseSlot;

    [Header("ƒополнительна€ иконка, например, запрет на хилл")]
    public Image noHealIcon;

    private List<ItemData> activeCurses = new();


    private Sprite originalSprite;
    private Color originalColor;

    private void Awake()
    {
        if (curseSlot != null)
        {
            originalSprite = curseSlot.sprite;
            originalColor = curseSlot.color;
        }
    }

    public void ApplyRandomCurse(GameObject player)
    {
        if (availableCurses.Count == 0) return;

        ItemData curse = GetRandomUnusedCurse();

        if (curse != null)
        {
            activeCurses.Add(curse);
            curse.effect?.Apply(player);

            if (curseSlot != null)
            {
                curseSlot.sprite = curse.icon;
                curseSlot.color = Color.white;
            }

            if (curse.effect.GetType().Name == "CurseNoHealing" && noHealIcon != null)
            {
                noHealIcon.gameObject.SetActive(true);
                noHealIcon.enabled = true;
            }

            Debug.Log("ѕрокл€тие применено: " + curse.itemName);
        }
    }

    public void RemoveAllCurses()
    {
        activeCurses.Clear();

        if (curseSlot != null)
        {
            curseSlot.sprite = originalSprite;
            curseSlot.color = originalColor;
        }

        if (noHealIcon != null)
        {
            noHealIcon.enabled = false;
            noHealIcon.gameObject.SetActive(false);
        }

        Debug.Log("¬се прокл€ти€ сн€ты.");
    }




    private ItemData GetRandomUnusedCurse()
    {
        List<ItemData> unused = availableCurses.FindAll(c => !activeCurses.Contains(c));
        if (unused.Count == 0) return null;
        return unused[Random.Range(0, unused.Count)];
    }
}
