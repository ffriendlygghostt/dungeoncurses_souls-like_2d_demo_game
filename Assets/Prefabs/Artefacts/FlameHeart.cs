using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Effects/FlameHeart")]
public class FlameHeart : ItemEffect
{
    public float igniteChance = 0.3f;
    public float burnDuration = 7.5f;

    public override void Apply(GameObject player)
    {
        if (!player.TryGetComponent<FlameHeartEffect>(out var effect))
        {
            effect = player.AddComponent<FlameHeartEffect>();
        }

        effect.igniteChance = igniteChance;
        effect.burnDuration = burnDuration;
    }

    public override void Remove(GameObject player)
    {
        var comp = player.GetComponent<FlameHeartEffect>();
        if (comp != null)
            Destroy(comp);
    }
}
