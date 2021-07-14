namespace CityBuilderCore
{
    public interface IGameSaver
    {
        /// <summary>
        /// check if the game is currently loading, used to suppress certain checks and initializations during loading
        /// </summary>
        bool IsLoading { get; }
        /// <summary>
        /// check if the game is currently saving
        /// </summary>
        bool IsSaving { get; }

        void Save();
        void Load();
    }
}