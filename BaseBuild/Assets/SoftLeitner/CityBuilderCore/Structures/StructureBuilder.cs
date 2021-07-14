using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// tool for building structures
    /// </summary>
    public class StructureBuilder : BaseTool
    {
        public StructureCollection Collection;
        public StructureTiles Tiles;
        public ItemQuantity[] Cost;

        private bool _isDragging;
        private Vector2Int _dragStart;

        private List<ItemQuantity> _costs = new List<ItemQuantity>();
        private IMouseInput _mouseInput;
        private IGlobalStorage _globalStorage;
        private IHighlightManager _highlighting;

        private void Start()
        {
            _mouseInput = Dependencies.Get<IMouseInput>();
            _globalStorage = Dependencies.Get<IGlobalStorage>();
            _highlighting = Dependencies.Get<IHighlightManager>();
        }

        public override void DeactivateTool()
        {
            base.DeactivateTool();

            _isDragging = false;
        }

        public override int GetCost(Item item)
        {
            return _costs.FirstOrDefault(c => c.Item == item)?.Quantity ?? 0;
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

            List<Vector2Int> validPositions = new List<Vector2Int>();
            List<Vector2Int> invalidPositions = new List<Vector2Int>();

            IEnumerable<Vector2Int> positions;

            if (_isDragging)
            {
                positions = PositionHelper.GetRoadPositions(_dragStart, mousePosition);
            }
            else
            {
                positions = new Vector2Int[] { mousePosition };
            }

            foreach (var position in positions)
            {
                if (Dependencies.Get<IStructureManager>().CheckAvailability(position, Collection ? Collection.Level.Value : Tiles.Level.Value) && Dependencies.Get<IMap>().IsInside(position))
                {
                    validPositions.Add(position);
                }
                else
                {
                    invalidPositions.Add(position);
                }
            }

            bool hasCost = true;
            _costs.Clear();
            foreach (var items in Cost)
            {
                _costs.Add(new ItemQuantity(items.Item, items.Quantity * validPositions.Count));

                if (!_globalStorage.Items.HasItems(items.Item, items.Quantity * validPositions.Count))
                {
                    hasCost = false;
                }
            }

            if (!hasCost)
            {
                invalidPositions.AddRange(validPositions);
                validPositions.Clear();
            }

            _highlighting.Clear();
            _highlighting.Highlight(validPositions, true);
            _highlighting.Highlight(invalidPositions, false);

            if (isUp)
            {
                _isDragging = false;

                if (validPositions.Any())
                    onApplied();

                foreach (var items in Cost)
                {
                    _globalStorage.Items.RemoveItems(items.Item, items.Quantity * validPositions.Count);
                }

                if (Collection)
                    Collection.Add(validPositions);
                if (Tiles)
                    Tiles.Add(validPositions);
            }
        }
    }
}