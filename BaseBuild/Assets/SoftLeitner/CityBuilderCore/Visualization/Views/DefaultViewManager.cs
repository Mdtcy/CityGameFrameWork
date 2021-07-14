using UnityEngine;

namespace CityBuilderCore
{
    public class DefaultViewManager : MonoBehaviour, IViewsManager
    {
        public bool HasActiveView => _activeView;
        public View ActiveView => _activeView;

        private View _activeView;

        protected virtual void Awake()
        {
            Dependencies.Register<IViewsManager>(this);
        }

        public void ActivateView(View view)
        {
            if (_activeView)
                _activeView.Deactivate();
            _activeView = view;
            if (_activeView)
                _activeView.Activate();
        }
        public void DeactivateView(View view)
        {
            if (_activeView != view)
                return;

            if (_activeView)
                _activeView.Deactivate();
            _activeView = null;
        }
    }
}