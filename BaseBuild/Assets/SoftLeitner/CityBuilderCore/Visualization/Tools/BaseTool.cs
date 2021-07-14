using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for tools that are managed by <see cref="ToolsManager"/><br/>
    /// gets activated, deactivated, has a cost, ...
    /// </summary>
    public abstract class BaseTool : MonoBehaviour
    {
        [Tooltip("gets de/activated along with the tool")]
        public View View;

        /// <summary>
        /// whether the <see cref="IGridOverlay"/> gets shown for this tool
        /// </summary>
        public virtual bool ShowGrid => true;

        public ToolEvent Activating;
        public ToolEvent Applied;

        private bool _hasActivatedView;
        private bool _isToolActive;

        private void Update()
        {
            if (_isToolActive && !EventSystem.current.IsPointerOverGameObject())
                updateTool();
        }

        public virtual void ActivateTool()
        {
            Activating?.Invoke(this);

            _isToolActive = true;
            if (ShowGrid)
                Dependencies.GetOptional<IGridOverlay>()?.Show();

            if (View && !Dependencies.Get<IViewsManager>().HasActiveView)
            {
                _hasActivatedView = true;
                Dependencies.Get<IViewsManager>().ActivateView(View);
            }
            else
            {
                _hasActivatedView = false;
            }
        }

        public virtual void DeactivateTool()
        {
            _isToolActive = false;
            if (ShowGrid)
                Dependencies.GetOptional<IGridOverlay>()?.Hide();

            if (_hasActivatedView && Dependencies.Get<IViewsManager>().ActiveView == View)
            {
                Dependencies.Get<IViewsManager>().ActivateView(null);
            }
        }

        public virtual int GetCost(Item item) => 0;

        protected void onApplied()
        {
            Applied?.Invoke(this);
        }

        protected virtual void updateTool()
        {
        }
    }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class ToolEvent : UnityEvent<BaseTool> { }
}