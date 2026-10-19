using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/HealFromBurn")]
public class HealFromBurn : ItemEffect
{
    public override void Apply(GameObject player)
    {
        if (!player.TryGetComponent<HealFromBurnEffect>(out _))
            player.AddComponent<HealFromBurnEffect>();
    }

    public override void Remove(GameObject player)
    {
        var comp = player.GetComponent<HealFromBurnEffect>();
        if (comp != null)
            Destroy(comp);
    }
}
