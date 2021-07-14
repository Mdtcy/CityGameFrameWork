using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// helper for activating a view from a <see cref="UnityEngine.UI.Toggle"/>
    /// </summary>
    public class ViewActivator : MonoBehaviour
    {
        public View View;

        public void SetViewActive(bool active)
        {
            var viewsManager = Dependencies.Get<IViewsManager>();

            if (active)
                viewsManager.ActivateView(View);
            else
                viewsManager.DeactivateView(View);
        }
    }
}