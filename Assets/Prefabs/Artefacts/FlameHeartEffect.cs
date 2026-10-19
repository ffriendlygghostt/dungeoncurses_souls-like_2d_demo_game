using UnityEngine;

public class FlameHeartEffect : MonoBehaviour, IAttackListener
{
    public float igniteChance = 0.3f;
    public float burnDuration = 7.5f;
    public int tickDamage = 6;
    public float tickInterval = 1.5f;

    public void OnAttack(GameObject target)
    {
        if (Random.value < igniteChance)
        {
            var burnable = GetComponent<Burnable>();
            if (burnable != null)
                burnable.Ignite();
        }
    }
}