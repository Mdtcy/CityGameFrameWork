namespace CityBuilderCore
{
    /// <summary>
    /// building component that stores items, how it does that can be configured in orders
    /// </summary>
    public interface IStorageComponent : IBuildingComponent, IItemReceiver, IItemGiver
    {
        ItemStorage Storage { get; }
        StorageOrder[] Orders { get; }
    }
}