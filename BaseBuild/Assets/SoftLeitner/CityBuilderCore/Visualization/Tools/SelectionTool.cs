using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// selects walkers and buildings under the mouse on click
    /// </summary>
    public class SelectionTool : BaseTool
    {
        public BuildingEvent BuildingSelected;
        public WalkerEvent WalkerSelected;

        public override bool ShowGrid => false;

        private IMouseInput _mouseInput;

        private void Start()
        {
            _mouseInput = Dependencies.Get<IMouseInput>();
        }

        protected override void updateTool()
        {
            base.updateTool();

            if (Input.GetMouseButtonUp(0))
            {
                var mousePosition = _mouseInput.GetMouseGridPosition();

                var walkerObject = Physics.RaycastAll(_mouseInput.GetRay()).Select(h => h.transform.gameObject).FirstOrDefault(g => g.CompareTag("Walker"));
                if (walkerObject)
                {
                    var walker = walkerObject.GetComponent<Walker>();
                    if (walker)
                    {
                        WalkerSelected?.Invoke(walker);
                        return;
                    }
                }

                var building = Dependencies.Get<IBuildingManager>().GetBuilding(mousePosition).FirstOrDefault();
                if (building != null)
                {
                    BuildingSelected?.Invoke(building.BuildingReference);
                    return;
                }

                onApplied();
            }
        }
    }
}