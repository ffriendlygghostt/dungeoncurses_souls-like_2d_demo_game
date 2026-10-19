using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/Frostmourne")]
public class Frostmourne : ItemEffect
{
    public override void Apply(GameObject player, ItemData item)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.AddGuaranteedEffect(PlayerStats.AttackType.Ice, item.numActivations);
        }
    }

    public override void Apply(GameObject player) { }
    public override void Remove(GameObject player) { }
}
