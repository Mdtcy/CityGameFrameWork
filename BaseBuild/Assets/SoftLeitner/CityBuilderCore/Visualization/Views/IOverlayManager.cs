namespace CityBuilderCore
{
    /// <summary>
    /// overlays the tilemap with coloured tiles that visualize <see cref="Layer"/> values<br/>
    /// eg desirable areas are green, undesirable areas are red
    /// </summary>
    public interface IOverlayManager
    {
        void ActivateOverlay(ViewLayer layerView);
        void ActivateOverlay(ViewEfficiency efficiencyView);
        void ClearOverlay();
    }
}