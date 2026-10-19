using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/CurseFireMan")]
public class CurseFireMan : ItemEffect
{
    public float fireChanceBonus = 30f;

    public override void Apply(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.AddFireChance(fireChanceBonus);
        }
    }

    public override void Remove(GameObject player)
    {
        var stats = player.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.AddFireChance(-fireChanceBonus);
        }
    }
}
