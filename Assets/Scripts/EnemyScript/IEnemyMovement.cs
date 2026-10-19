using UnityEngine;

public interface IEnemyMovement
{
    void Move();
    void SetTarget(Transform target);
    void Initialize(EnemyStats stats);
    void SetRunning(bool running);
    void FreezeMovement(bool canmove);
    void StartHurtFreeze(float duration);
    void SetSpeedMultiplier(float speed);
    void StartAttackFreeze(float duration);
    public bool IsFrozen { get; }
    public bool IsFlipped { get; }

    public bool IsFrozenByHurt { get; }
}