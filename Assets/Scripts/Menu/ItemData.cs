using UnityEngine;

public enum ItemType
{
    Weapon,
    Armor,
    BackPack,
    Activity,
    Book,
    Curse
}

public abstract class ItemEffect : ScriptableObject
{
    public abstract void Apply(GameObject player);
    public virtual void Apply(GameObject player, ItemData item)
    {
        Apply(player);
    }
    public abstract void Remove(GameObject player);
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea]
    public string description;
    public Sprite icon;
    public ItemType type;
    public ItemEffect effect;
    public int price;
    public Color colorBack = Color.clear;
    public float cooldown;
    public int numActivations;
    public string keyTag = ""; // ← например: "SecretDoorKey"


}
