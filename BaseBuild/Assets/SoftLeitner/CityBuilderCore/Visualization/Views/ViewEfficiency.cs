using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// view that displays an overlay on a tilemap
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Views/" + nameof(ViewEfficiency))]
    public class ViewEfficiency : View
    {
        public Gradient Gradient;

        public override void Activate() => Dependencies.Get<IOverlayManager>().ActivateOverlay(this);
        public override void Deactivate() => Dependencies.Get<IOverlayManager>().ClearOverlay();
    }
}
