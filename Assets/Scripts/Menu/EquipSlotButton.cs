using UnityEngine;
using UnityEngine.EventSystems;

public class EquipSlotButton : MonoBehaviour, IPointerClickHandler
{
    public InventoryManager inventoryManager;
    public ItemType slotType;
    public int slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryManager.UnequipFromSlot(slotType, slotIndex);
        }
    }
}
