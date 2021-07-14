using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// view that displays an overlay on a tilemap
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Views/" + nameof(ViewLayer))]
    public class ViewLayer : View
    {
        public Layer Layer;
        public Gradient Gradient;
        public int Minimum;
        public int Maximum;

        public override void Activate() => Dependencies.Get<IOverlayManager>().ActivateOverlay(this);
        public override void Deactivate() => Dependencies.Get<IOverlayManager>().ClearOverlay();
    }
}