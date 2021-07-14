namespace CityBuilderCore
{
    /// <summary>
    /// manages the tool activation and tool cost
    /// </summary>
    public interface IToolsManager
    {
        void ActivateTool(BaseTool tool);
        void DeactivateTool(BaseTool tool);
        int GetCost(Item item);
    }
}