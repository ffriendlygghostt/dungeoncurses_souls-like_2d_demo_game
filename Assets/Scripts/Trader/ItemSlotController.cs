using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public GameObject border;
    public Button selectButton;

    public ItemData CurrentItem { get; private set; }

    private TradePanelManager manager;

    public void Setup(ItemData newItem, TradePanelManager tradeManager)
    {
        CurrentItem = newItem;
        manager = tradeManager;

        UpdateVisual();

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(OnSlotClicked);

        Highlight(false);
    }

    private void OnSlotClicked()
    {
        manager?.SelectItem(CurrentItem, this);
    }

    private void UpdateVisual()
    {
        if (CurrentItem != null && CurrentItem.icon != null)
        {
            iconImage.sprite = CurrentItem.icon;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.color = new Color(1f, 1f, 1f, 0.025f);
        }
    }

    public void Highlight(bool state)
    {
        if (border != null)
            border.SetActive(state);
    }

    public void ClearSlot()
    {
        Setup(null, manager);
    }
}
