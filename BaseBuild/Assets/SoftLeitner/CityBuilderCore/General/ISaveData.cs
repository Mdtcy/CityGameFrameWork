namespace CityBuilderCore
{
    /// <summary>
    /// interface for all kinds of components that contain state that needs saving
    /// </summary>
    public interface ISaveData
    {
        string SaveData();
        void LoadData(string json);
    }
}