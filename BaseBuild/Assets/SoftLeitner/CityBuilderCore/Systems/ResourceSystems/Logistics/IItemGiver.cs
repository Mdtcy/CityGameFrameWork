namespace CityBuilderCore
{
    /// <summary>
    /// building component that provides others with items
    /// </summary>
    public interface IItemGiver : IBuildingTrait<IItemGiver>
    {
        bool HasItems(Item item, int amount);
        void Reserve(Item item, int amount);
        void Unreserve(Item item, int amount);
        void Give(ItemStorage storage, Item item, int amount);
    }
}