namespace CityBuilderCore
{
    /// <summary>
    /// building component that needs to be supplied with items by others(eg production buildings that get supplied by storage)
    /// </summary>
    public interface IItemReceiver : IBuildingTrait<IItemReceiver>
    {
        int Priority { get; }

        int GetItemCapacityRemaining(Item item);
        void Receive(ItemStorage storage);
    }
}