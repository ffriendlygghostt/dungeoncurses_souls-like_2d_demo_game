public interface IBurnReaction
{
    /// <summary>
    /// Возвращает true, если кастомная реакция сработала, и стандартный урон не нужен.
    /// </summary>
    bool OnBurnTick(Burnable burnable, ref int damage);
}
