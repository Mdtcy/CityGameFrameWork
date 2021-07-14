using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// tool for placing buildings
    /// </summary>
    public class BuildingBuilder : BaseTool
    {
        public BuildingInfo BuildingInfo;
        public bool AllowRotate = true;

        private bool _isDown;
        private Vector2Int _dragStart;
        private BuildingRotation _rotation;
        private GameObject _ghost;

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

            _rotation = new BuildingRotation();

            if (BuildingInfo.Ghost)
            {
                _ghost = Instantiate(BuildingInfo.Ghost);
                _ghost.SetActive(false);
            }
        }

        public override void DeactivateTool()
        {
            if (_ghost)
            {
                Destroy(_ghost);
            }

            _costs.Clear();
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

            if (!_isDown)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    _rotation.TurnClockwise();
                }
            }

            List<Vector2Int> validPoints = new List<Vector2Int>();
            List<Vector2Int> invalidPoints = new List<Vector2Int>();

            Vector2Int size = _rotation.RotateSize(BuildingInfo.Size);
            List<Vector2Int> buildPoints = _isDown ? PositionHelper.GetBoxPositions(_dragStart, mousePoint, size).ToList() : new List<Vector2Int>() { mousePoint };
            List<Vector2Int> validBuildPoints = new List<Vector2Int>();

            foreach (var buildPoint in buildPoints)
            {
                var structurePoints = PositionHelper.GetStructurePositions(buildPoint, size);
                var isInside = structurePoints.All(p => _map.IsInside(p));
                var isFulfillingRequirements = isInside && BuildingInfo.CheckBuildingRequirements(buildPoint, _rotation);
                var isCompletelyValid = true;

                foreach (var point in structurePoints)
                {
                    if (isFulfillingRequirements && BuildingInfo.CheckBuildingAvailability(point))
                    {
                        validPoints.Add(point);
                    }
                    else
                    {
                        invalidPoints.Add(point);
                        isCompletelyValid = false;
                    }
                }

                if (isCompletelyValid)
                {
                    validBuildPoints.Add(buildPoint);
                }

                if (buildPoints.IndexOf(buildPoint) == 0 && _ghost)
                {
                    _ghost.SetActive(isCompletelyValid);
                    _ghost.transform.position = Dependencies.Get<IGridPositions>().GetWorldPosition(_rotation.RotateOrigin(buildPoint, BuildingInfo.Size));
                    _ghost.transform.rotation = _rotation.GetRotation();
                }
            }

            if (!checkCost(Mathf.Max(1, validBuildPoints.Count)))
            {
                invalidPoints.AddRange(validPoints);
                validPoints.Clear();
                validBuildPoints.Clear();
            }

            _highlighting.Clear();
            if (BuildingInfo.AccessType != BuildingAccessType.Any)
                _highlighting.Highlight(_rotation.RotateBuildingPoint(mousePoint, BuildingInfo.AccessPoint, BuildingInfo.Size), HighlightType.Info);
            _highlighting.Highlight(validPoints, true);
            _highlighting.Highlight(invalidPoints, false);

            if (Input.GetMouseButtonUp(0))
            {
                _isDown = false;
                build(validBuildPoints);
            }
        }

        private bool checkCost(int count)
        {
            bool hasCost = true;
            _costs.Clear();
            foreach (var items in BuildingInfo.Cost)
            {
                _costs.Add(new ItemQuantity(items.Item, items.Quantity * count));
                if (_globalStorage != null && !_globalStorage.Items.HasItems(items.Item, items.Quantity * count))
                {
                    hasCost = false;
                }
            }
            return hasCost;
        }

        private void build(IEnumerable<Vector2Int> points)
        {
            var buildingManager = Dependencies.Get<IBuildingManager>();
            var gridPositions = Dependencies.Get<IGridPositions>();

            if (points.Any())
                onApplied();

            foreach (var point in points)
            {
                if (_globalStorage != null)
                {
                    foreach (var items in BuildingInfo.Cost)
                    {
                        _globalStorage.Items.RemoveItems(items.Item, items.Quantity);
                    }
                }

                BuildingInfo.Prepare(point, _rotation);

                buildingManager.Add(gridPositions.GetWorldPosition(_rotation.RotateOrigin(point, BuildingInfo.Size)), _rotation.GetRotation(), BuildingInfo.Prefab);
            }
        }
    }
}