using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Right-Side Slots")]
    public Image[] weaponSlots;
    public Image[] armorSlots;
    public Image[] backPackSlots;
    public Image[] activitySlots;
    public Image[] bookSlots;

    [Header("Scroll View")]
    public Transform itemListContent; // Контейнер для скролла
    public GameObject itemPanelPrefab;

    private List<GameObject> itemPanels = new();
    private List<ItemData> inventoryItems = new();

    private ItemData[] equippedWeapons;
    private ItemData[] equippedArmors;
    private ItemData[] equippedBackPacks;
    private ItemData[] equippedActivities;
    private ItemData[] equippedBooks;

    [Header("Links")]
    public GameObject player;
    public ActivityManager activityManager;


    void Start()
    {
        equippedWeapons = new ItemData[weaponSlots.Length];
        equippedArmors = new ItemData[armorSlots.Length];
        equippedBackPacks = new ItemData[backPackSlots.Length];
        equippedActivities = new ItemData[activitySlots.Length];
        equippedBooks = new ItemData[bookSlots.Length];


        if (activityManager == null)
        {
            activityManager = FindObjectOfType<ActivityManager>();
        }
        else { Debug.LogWarning("ActivityManager: null"); }

        ClearAllSlots();
        RefreshInventoryPanels();
        RefreshEquipSlots();
    }

    void ClearAllSlots()
    {
        foreach (var slot in weaponSlots)
            if (slot) { slot.sprite = null; slot.color = new Color(1, 1, 1, 0.3f); }

        foreach (var slot in armorSlots)
            if (slot) { slot.sprite = null; slot.color = new Color(1, 1, 1, 0.3f); }

        foreach (var slot in backPackSlots)
            if (slot) { slot.sprite = null; slot.color = new Color(1, 1, 1, 0.3f); }

        foreach (var slot in activitySlots)
            if (slot) { slot.sprite = null; slot.color = new Color(1, 1, 1, 0.3f); }

        foreach (var slot in bookSlots)
            if (slot) { slot.sprite = null; slot.color = new Color(1, 1, 1, 0.3f); }

    }

    public bool AddItem(ItemData newItem)
    {
        if (newItem == null) return false;
        inventoryItems.Add(newItem);
        RefreshInventoryPanels();
        return true;
    }


    public bool HasItemWithKeyTag(string tag)
    {
        // Проверка в инвентаре
        foreach (var item in inventoryItems)
        {
            if (item != null && item.keyTag == tag)
                return true;
        }

        // Проверка во всех экипированных слотах (безопасно)
        ItemData[][] allEquipped =
        {
        equippedWeapons,
        equippedArmors,
        equippedBackPacks,
        equippedActivities,
        equippedBooks
    };

        foreach (var group in allEquipped)
        {
            if (group == null) continue; // ⛑️ защита от null массива

            foreach (var item in group)
            {
                if (item != null && item.keyTag == tag)
                    return true;
            }
        }

        return false;
    }

    public void RemoveItemByKeyTag(string tag)
    {
        // Удалить из инвентаря
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i] != null && inventoryItems[i].keyTag == tag)
            {
                inventoryItems.RemoveAt(i);
                RefreshInventoryPanels();
                return;
            }
        }

        // Удалить из экипировки (если ключ экипирован)
        ItemData[][] allEquipped =
        {
        equippedWeapons,
        equippedArmors,
        equippedBackPacks,
        equippedActivities,
        equippedBooks
    };

        for (int g = 0; g < allEquipped.Length; g++)
        {
            var group = allEquipped[g];
            if (group == null) continue;

            for (int i = 0; i < group.Length; i++)
            {
                var item = group[i];
                if (item != null && item.keyTag == tag)
                {
                    group[i] = null;
                    RefreshEquipSlots();
                    return;
                }
            }
        }
    }






    public void EquipFromInventory(ItemData item)
    {
        bool equipped = false;
        switch (item.type)
        {
            case ItemType.Weapon:
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    if (equippedWeapons[i] == null)
                    {
                        SetSlot(weaponSlots[i], item);
                        equippedWeapons[i] = item;
                        equipped = true;
                        break;
                    }
                }
                break;
            case ItemType.Armor:
                for (int i = 0; i < armorSlots.Length; i++)
                {
                    if (equippedArmors[i] == null)
                    {
                        SetSlot(armorSlots[i], item);
                        equippedArmors[i] = item;
                        equipped = true;
                        break;
                    }
                }
                break;
            case ItemType.BackPack:
                for (int i = 0; i < backPackSlots.Length; i++)
                {
                    if (equippedBackPacks[i] == null)
                    {
                        SetSlot(backPackSlots[i], item);
                        equippedBackPacks[i] = item;
                        equipped = true;
                        break;
                    }
                }
                break;
            case ItemType.Activity:
                for (int i = 0; i < activitySlots.Length; i++)
                {
                    if (equippedActivities[i] == null)
                    {
                        SetSlot(activitySlots[i], item);
                        equippedActivities[i] = item;
                        equipped = true;

                        if (activityManager != null)
                            activityManager.SetActivitySlot(i, item);

                        break;
                    }
                }
                break;
            case ItemType.Book:
                for (int i = 0; i < bookSlots.Length; i++)
                {
                    if (equippedBooks[i] == null)
                    {
                        SetSlot(bookSlots[i], item);
                        equippedBooks[i] = item;
                        equipped = true;
                        break;
                    }
                }
                break;

        }

        if (equipped)
        {
            inventoryItems.Remove(item);
            RefreshInventoryPanels();
            RefreshEquipSlots();

            if (item.type != ItemType.Activity)
                item.effect?.Apply(player);

        }


    }

    public void UnequipFromSlot(ItemType type, int slotIndex)
    {
        ItemData removed = null;
        switch (type)
        {
            case ItemType.Weapon:
                removed = equippedWeapons[slotIndex];
                equippedWeapons[slotIndex] = null;
                SetSlot(weaponSlots[slotIndex], null);
                break;
            case ItemType.Armor:
                removed = equippedArmors[slotIndex];
                equippedArmors[slotIndex] = null;
                SetSlot(armorSlots[slotIndex], null);
                break;
            case ItemType.BackPack:
                removed = equippedBackPacks[slotIndex];
                equippedBackPacks[slotIndex] = null;
                SetSlot(backPackSlots[slotIndex], null);
                break;
            case ItemType.Activity:
                removed = equippedActivities[slotIndex];
                equippedActivities[slotIndex] = null;
                SetSlot(activitySlots[slotIndex], null);

                if (activityManager != null)
                    activityManager.SetActivitySlot(slotIndex, null);

                break;
            case ItemType.Book:
                removed = equippedBooks[slotIndex];
                equippedBooks[slotIndex] = null;
                SetSlot(bookSlots[slotIndex], null);
                break;
        }
        if (removed != null)
        {
            if (removed.type != ItemType.Activity)
                removed.effect?.Remove(player);


            inventoryItems.Add(removed);
            RefreshInventoryPanels();
            RefreshEquipSlots();
        }
    }

    void SetSlot(Image slot, ItemData item)
    {
        if (slot != null)
        {
            if (item != null)
            {
                slot.sprite = item.icon;
                slot.color = Color.white;
            }
            else
            {
                slot.sprite = null;
                slot.color = new Color(1, 1, 1, 0.3f);
            }
        }
    }

    void RefreshInventoryPanels()
    {
        // Удалить старые панели
        foreach (var panel in itemPanels)
            Destroy(panel);
        itemPanels.Clear();

        // Добавить новые панели под актуальный список
        foreach (var item in inventoryItems)
        {
            GameObject panel = Instantiate(itemPanelPrefab, itemListContent);
            FillListPanel(panel, item);

            // Установить InventoryItemButton, если он есть на префабе
            InventoryItemButton btn = panel.GetComponent<InventoryItemButton>();
            if (btn != null)
            {
                btn.inventoryManager = this;
                btn.itemData = item;
            }
            itemPanels.Add(panel);
        }

    }

    void RefreshEquipSlots()
    {
        // Обновить визуальное состояние всех слотов (на случай сброса/снятия)
        for (int i = 0; i < weaponSlots.Length; i++)
            SetSlot(weaponSlots[i], i < equippedWeapons.Length ? equippedWeapons[i] : null);
        for (int i = 0; i < armorSlots.Length; i++)
            SetSlot(armorSlots[i], i < equippedArmors.Length ? equippedArmors[i] : null);
        for (int i = 0; i < backPackSlots.Length; i++)
            SetSlot(backPackSlots[i], i < equippedBackPacks.Length ? equippedBackPacks[i] : null);
        for (int i = 0; i < activitySlots.Length; i++)
            SetSlot(activitySlots[i], i < equippedActivities.Length ? equippedActivities[i] : null);
        for (int i = 0; i < bookSlots.Length; i++)
            SetSlot(bookSlots[i], i < equippedBooks.Length ? equippedBooks[i] : null);

    }

    void FillListPanel(GameObject panel, ItemData item)
    {
        if (panel == null || item == null) return;

        Image iconImage = panel.transform.Find("Icon")?.GetComponent<Image>();
        TMP_Text nameText = panel.transform.Find("Name")?.GetComponent<TMP_Text>();
        TMP_Text descText = panel.transform.Find("Describ")?.GetComponent<TMP_Text>();
        Image nameBackImage = panel.transform.Find("NameBack")?.GetComponent<Image>();

        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
            iconImage.color = Color.white;
        }

        if (nameText != null)
            nameText.text = item.itemName;

        if (descText != null)
            descText.text = item.description;

        if (nameBackImage != null && item.colorBack.a > 0.01f)
            nameBackImage.color = item.colorBack;
    }
}
