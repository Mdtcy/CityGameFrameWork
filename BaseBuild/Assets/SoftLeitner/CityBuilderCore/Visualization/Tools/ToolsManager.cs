using UnityEngine;
using UnityEngine.UI;

namespace CityBuilderCore
{
    public class ToolsManager : MonoBehaviour, IToolsManager
    {
        public BaseTool FallbackTool;
        public ToggleGroup ToggleGroup;

        private BaseTool _activeTool;
        private IHighlightManager _highlighting;

        protected virtual void Awake()
        {
            Dependencies.Register<IToolsManager>(this);
        }

        private void Start()
        {
            _highlighting = Dependencies.Get<IHighlightManager>();

            if (FallbackTool)
                ActivateTool(FallbackTool);
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(1) && _activeTool)
            {
                ToggleGroup.SetAllTogglesOff(false);
                DeactivateTool(_activeTool);
            }
        }

        public void ActivateTool(BaseTool tool)
        {
            if (_activeTool)
                _activeTool.DeactivateTool();
            _highlighting.Clear();
            _activeTool = tool ? tool : FallbackTool;
            if (_activeTool)
                _activeTool.ActivateTool();
        }

        public void DeactivateTool(BaseTool tool)
        {
            if (_activeTool != tool)
                return;

            ActivateTool(null);
        }

        public int GetCost(Item item) => _activeTool ? _activeTool.GetCost(item) : 0;
    }
}