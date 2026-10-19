
using UnityEngine;

public interface IIgnitable
{
    void Ignite();
    void Ignite(GameObject source, float duration, float tickInterval, int tickDamage);
    void Extinguish();
    bool IsBurning { get; }
}
