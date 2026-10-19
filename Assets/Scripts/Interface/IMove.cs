public interface IMove
{
    void SetSpeedMultiplier(float multiplier); 
    void SetDashMultiplier(float multiplier);  
    void FreezeMovement(bool freeze);
    void CanMove(bool move);
    void SetCanWalk(bool value);
    void SetCanDash(bool value);
    void FreezeMovementNotSetAnim(bool freeze);
}
