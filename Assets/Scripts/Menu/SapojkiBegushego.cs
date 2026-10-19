using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/SapojkiBegushego")]
public class SapojkiBegushego : ItemEffect
{
    public float speedBuff = 1f; // +100%

    public override void Apply(GameObject player)
    {
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.SetSpeedBuff(speedBuff);
        }
    }

    public override void Remove(GameObject player)
    {
        var movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.SetSpeedBuff(-speedBuff);
        }
    }
}
