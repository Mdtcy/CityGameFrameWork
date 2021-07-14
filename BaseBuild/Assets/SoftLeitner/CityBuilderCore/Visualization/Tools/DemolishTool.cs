using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// tool that removes structures
    /// </summary>
    public class DemolishTool : BaseTool
    {
        [Tooltip("optional effect that gets added for removed buildings")]
        public DemolishVisual Visual;
        [Tooltip("determines which structures are affected by this tool")]
        public StructureLevelMask Level;

        private bool _isDragging;
        private Vector2Int _dragStart;
        private IMouseInput _mouseInput;
        private IHighlightManager _highlighting;

        private void Start()
        {
            _mouseInput = Dependencies.Get<IMouseInput>();
            _highlighting = Dependencies.Get<IHighlightManager>();
        }

        public override void DeactivateTool()
        {
            base.DeactivateTool();

            _isDragging = false;
        }

        protected override void updateTool()
        {
            base.updateTool();

            bool isDown = Input.GetMouseButtonDown(0);
            bool isUp = Input.GetMouseButtonUp(0);

            var mousePosition = _mouseInput.GetMouseGridPosition();

            if (isDown)
            {
                _isDragging = true;
                _dragStart = mousePosition;
            }

            _highlighting.Clear();

            if (_isDragging)
            {
                _highlighting.Highlight(PositionHelper.GetBoxPositions(_dragStart, mousePosition), false);
            }
            else
            {
                _highlighting.Highlight(mousePosition, false);
            }

            if (isUp)
            {
                _isDragging = false;

                int count = Dependencies.Get<IStructureManager>().Remove(PositionHelper.GetBoxPositions(_dragStart, mousePosition), Level.Value, false, structure =>
                   {
                       DemolishVisual.Create(Visual, structure as IBuilding);
                   });

                if (count > 0)
                    onApplied();
            }
        }
    }
}