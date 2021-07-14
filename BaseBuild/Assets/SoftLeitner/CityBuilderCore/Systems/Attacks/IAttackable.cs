namespace CityBuilderCore
{
    /// <summary>
    /// a building component that can be attacked
    /// </summary>
    public interface IAttackable : IBuildingTrait<IAttackable>
    {
        void Attack(int damage);
    }
}