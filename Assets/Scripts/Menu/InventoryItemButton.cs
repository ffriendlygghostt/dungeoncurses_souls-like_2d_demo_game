using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemButton : MonoBehaviour, IPointerClickHandler
{
    public InventoryManager inventoryManager;
    public ItemData itemData;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryManager.EquipFromInventory(itemData);
        }
    }
}
