namespace CityBuilderCore
{
    /// <summary>
    /// manages the active <see cref="View"/>
    /// </summary>
    public interface IViewsManager
    {
        View ActiveView { get; }
        bool HasActiveView { get; }

        void ActivateView(View view);
        void DeactivateView(View view);
    }
}