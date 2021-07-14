using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    public class ExpandableBuilder : BaseTool
    {
        public ExpandableBuildingInfo BuildingInfo;

        private bool _isDown;
        private Vector2Int _dragStart;
        private ExpandableVisual _ghost;

        private List<ItemQuantity> _costs = new List<ItemQuantity>();
        private IMouseInput _mouseInput;
        private IGlobalStorage _globalStorage;
        private IHighlightManager _highlighting;
        private IMap _map;

        private void Start()
        {
            _mouseInput = Dependencies.Get<IMouseInput>();
            _globalStorage = Dependencies.GetOptional<IGlobalStorage>();
            _highlighting = Dependencies.Get<IHighlightManager>();
            _map = Dependencies.Get<IMap>();
        }

        public override void ActivateTool()
        {
            base.ActivateTool();

            if (BuildingInfo.Ghost)
            {
                _ghost = Instantiate(BuildingInfo.Ghost).GetComponent<ExpandableVisual>();
                _ghost.gameObject.SetActive(false);
            }
        }

        public override void DeactivateTool()
        {
            if (_ghost)
            {
                Destroy(_ghost);
            }

            _isDown = false;

            base.DeactivateTool();
        }

        public override int GetCost(Item item)
        {
            return _costs.FirstOrDefault(c => c.Item == item)?.Quantity ?? 0;
        }

        protected override void updateTool()
        {
            base.updateTool();

            var mousePoint = _mouseInput.GetMouseGridPosition();

            if (Input.GetMouseButtonDown(0))
            {
                _isDown = true;
                _dragStart = mousePoint;
            }

            _highlighting.Clear();

            if (_isDown)
            {
                List<Vector2Int> validPoints = new List<Vector2Int>();
                List<Vector2Int> invalidPoints = new List<Vector2Int>();

                Vector2Int drag = mousePoint - _dragStart;

                Vector2Int expansion;
                Vector2Int point;
                BuildingRotation rotation;

                if (BuildingInfo.IsArea)
                {
                    expansion = new Vector2Int(Mathf.Abs(drag.x), Mathf.Abs(drag.y)) - BuildingInfo.Size - BuildingInfo.SizePost + Vector2Int.one;

                    if (drag.x >= 0)
                    {
                        if (drag.y >= 0)
                        {
                            point = _dragStart;
                            rotation = new BuildingRotation(0);
                        }
                        else
                        {
                            expansion = new Vector2Int(expansion.y, expansion.x);
                            point = new Vector2Int(_dragStart.x, mousePoint.y);
                            rotation = new BuildingRotation(1);
                        }
                    }
                    else
                    {
                        if (drag.y >= 0)
                        {
                            expansion = new Vector2Int(expansion.y, expansion.x);
                            point = new Vector2Int(mousePoint.x, _dragStart.y);
                            rotation = new BuildingRotation(3);
                        }
                        else
                        {
                            point = mousePoint;
                            rotation = new BuildingRotation(2);
                        }
                    }
                }
                else
                {
                    if (Mathf.Abs(drag.x) >= Mathf.Abs(drag.y))
                    {
                        expansion = new Vector2Int(Mathf.Abs(drag.x) - BuildingInfo.Size.x - BuildingInfo.SizePost.x + 1, 0);
                        point = drag.x > 0 ? _dragStart : new Vector2Int(mousePoint.x, _dragStart.y);
                        rotation = drag.x >= 0 ? new BuildingRotation(0) : new BuildingRotation(2);
                    }
                    else
                    {
                        expansion = new Vector2Int(Mathf.Abs(drag.y) - BuildingInfo.Size.x - BuildingInfo.SizePost.x + 1, 0);
                        point = drag.y > 0 ? _dragStart : new Vector2Int(_dragStart.x, mousePoint.y);
                        rotation = drag.y >= 0 ? new BuildingRotation(1) : new BuildingRotation(3);
                    }
                }

                var size = BuildingInfo.Size + expansion + BuildingInfo.SizePost;
                var structurePoints = PositionHelper.GetStructurePositions(point, rotation.RotateSize(size));

                if (structurePoints.All(p => _map.IsInside(p)) && BuildingInfo.CheckExpansionLimits(expansion) && BuildingInfo.CheckExpandedBuildingRequirements(point, expansion, rotation))
                {
                    foreach (var structurePoint in structurePoints)
                    {
                        if (BuildingInfo.CheckBuildingAvailability(structurePoint))
                            validPoints.Add(structurePoint);
                        else
                            invalidPoints.Add(structurePoint);
                    }
                }
                else
                {
                    invalidPoints.AddRange(structurePoints);
                }

                if (!checkCost(expansion))
                {
                    invalidPoints.AddRange(validPoints);
                    validPoints.Clear();
                }

                _highlighting.Highlight(validPoints, true);
                _highlighting.Highlight(invalidPoints, false);

                if (_ghost)
                {
                    _ghost.gameObject.SetActive(BuildingInfo.CheckExpansionLimits(expansion));
                    _ghost.transform.position = Dependencies.Get<IGridPositions>().GetWorldPosition(rotation.RotateOrigin(point, size));
                    _ghost.transform.rotation = rotation.GetRotation();
                    _ghost.UpdateVisual(expansion);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    _isDown = false;
                    if (validPoints.Count > 0 && invalidPoints.Count == 0)
                        build(point, rotation, expansion);
                }
            }
            else
            {
                _highlighting.Highlight(mousePoint, false);

                if (_ghost)
                    _ghost.gameObject.SetActive(false);
            }
        }

        private bool checkCost(Vector2Int expansion)
        {
            bool hasCost = true;
            _costs.Clear();

            foreach (var items in BuildingInfo.Cost)
            {
                _costs.AddQuantity(items.Item, items.Quantity);
                if (_globalStorage != null && !_globalStorage.Items.HasItems(items.Item, items.Quantity))
                {
                    hasCost = false;
                }
            }

            var expansionCount = getExpansionCount(expansion);
            foreach (var items in BuildingInfo.ExpansionCost)
            {
                _costs.AddQuantity(items.Item, items.Quantity * expansionCount);
                if (_globalStorage != null && !_globalStorage.Items.HasItems(items.Item, items.Quantity * expansionCount))
                {
                    hasCost = false;
                }
            }

            return hasCost;
        }

        private int getExpansionCount(Vector2Int expansion) => expansion.y == 0 ? expansion.x : expansion.x * expansion.y;

        private void build(Vector2Int point, BuildingRotation rotation, Vector2Int expansion)
        {
            var buildingManager = Dependencies.Get<IBuildingManager>();
            var gridPositions = Dependencies.Get<IGridPositions>();
            var size = BuildingInfo.Size + expansion + BuildingInfo.SizePost;

            onApplied();

            if (_globalStorage != null)
            {
                foreach (var items in BuildingInfo.Cost)
                {
                    _globalStorage.Items.RemoveItems(items.Item, items.Quantity);
                }

                var expansionCount = getExpansionCount(expansion);
                foreach (var items in BuildingInfo.ExpansionCost)
                {
                    _globalStorage.Items.RemoveItems(items.Item, items.Quantity * expansionCount);
                }
            }


            BuildingInfo.PrepareExpanded(point, expansion, rotation);
            buildingManager.Add(gridPositions.GetWorldPosition(rotation.RotateOrigin(point, size)), rotation.GetRotation(), BuildingInfo.Prefab, b => ((ExpandableBuilding)b).Expansion = expansion);
        }
    }
}
