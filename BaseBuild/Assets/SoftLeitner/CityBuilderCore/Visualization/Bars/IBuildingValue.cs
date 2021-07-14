namespace CityBuilderCore
{
    /// <summary>
    /// any kind of value a building might have(risks, services,...)
    /// </summary>
    public interface IBuildingValue
    {
        bool HasValue(IBuilding building);
        float GetValue(IBuilding building);
    }
}