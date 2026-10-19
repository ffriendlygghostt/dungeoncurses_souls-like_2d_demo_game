using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryListItem : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descText;

    public void SetItem(ItemData item)
    {
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        nameText.text = item.itemName;
        descText.text = item.description;
    }
}
