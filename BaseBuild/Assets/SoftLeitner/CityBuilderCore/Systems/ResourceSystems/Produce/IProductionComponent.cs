namespace CityBuilderCore
{
    /// <summary>
    /// building component that produces items from other items
    /// </summary>
    public interface IProductionComponent : IProgressComponent, IItemReceiver
    {
        ItemConsumer[] Consumers { get; }
        ItemProducer[] Producers { get; }
    }
}