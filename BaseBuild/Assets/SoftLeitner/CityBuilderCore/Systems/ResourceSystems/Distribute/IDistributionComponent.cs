namespace CityBuilderCore
{
    /// <summary>
    /// a building component that gets items, stores them and then distributes them to <see cref="ItemRecipient"/>
    /// </summary>
    public interface IDistributionComponent : IBuildingComponent
    {
        ItemStorage Storage { get; }
        DistributionOrder[] Orders { get; }
    }
}