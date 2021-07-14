namespace CityBuilderCore
{
    /// <summary>
    /// a building component that generates items
    /// </summary>
    public interface IGenerationComponent : IProgressComponent
    {
        ItemProducer[] ItemsProducers { get; }

        void Collect(ItemStorage storage, Item[] items);
    }
}