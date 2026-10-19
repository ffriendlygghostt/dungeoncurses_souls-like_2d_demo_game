using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    void ApplyDamage(int dmg);
    void ApplyDamage(int amount, Color color);
    void Resurrection();
    int CurrentHealth { get; }
    bool IsDead { get; }

    void Heal(int heals);
   

    void ParryOn();
}
