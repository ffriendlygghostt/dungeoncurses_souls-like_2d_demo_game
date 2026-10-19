using UnityEngine;

public class HealFromBurnEffect : MonoBehaviour, IBurnReaction
{
    private IHealth health;

    private void Awake()
    {
        health = GetComponent<IHealth>();
    }

    public bool OnBurnTick(Burnable burnable, ref int damage)
    {
        health?.Heal(damage); 
        return true;
    }
}
