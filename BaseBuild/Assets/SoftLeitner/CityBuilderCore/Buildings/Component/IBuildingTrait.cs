namespace CityBuilderCore
{
    /// <summary>
    /// traits are special kinds of components that are registered inside <see cref="IBuildingManager"/> for easy access<br/>
    /// this makes it easy to retrieve for example all storages or worker users
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuildingTrait<T> : IBuildingComponent where T : IBuildingTrait<T>
    {
        BuildingComponentReference<T> Reference { get; set; }
    }
}