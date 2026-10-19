using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/CurseNoHealing")]
public class CurseNoHealing : ItemEffect
{
    public override void Apply(GameObject player)
    {
        var book = player.GetComponent<HealingBook>();
        if (book != null)
        {
            book.CantHeal();
        }
    }

    public override void Remove(GameObject player)
    {
        var book = player.GetComponent<HealingBook>();
        if (book != null)
        {
            book.CanHeal();
        }
    }
}
