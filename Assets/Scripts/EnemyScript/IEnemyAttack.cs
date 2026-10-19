public interface IEnemyAttack
{
    void TryAttack();
    void Initialize(EnemyStats stats);
    void CanAttack(bool canattack);
}
