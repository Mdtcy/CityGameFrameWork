using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for walkers that roam and perform actions when passing buildings
    /// </summary>
    public abstract class BuildingWalker : RoamingWalker
    {
        public int Area = 1;
        public bool IsCross;

        private readonly List<BuildingReference> _buildings = new List<BuildingReference>();

        private void Update()
        {
            _buildings.Where(b => b.HasInstance).ForEach(b => onRemaining(b.Instance));
        }

        public override void Initialize(BuildingReference home, Vector2Int start)
        {
            base.Initialize(home, start);

            _buildings.Clear();
        }

        public override void Spawned()
        {
            base.Spawned();

            checkBuildings();
        }

        protected override void onMoved(Vector2Int position)
        {
            base.onMoved(position);

            checkBuildings();
        }

        protected virtual void onEntered(IBuilding building)
        {

        }
        protected virtual void onRemaining(IBuilding building)
        {

        }

        private void checkBuildings()
        {
            var exited = _buildings.ToList();
            var buildingManager = Dependencies.Get<IBuildingManager>();
            foreach (var areaPosition in IsCross ? PositionHelper.GetAreaCrossPositions(CurrentPoint, Area) : PositionHelper.GetAreaAroundPositions(CurrentPoint, Area))
            {
                foreach (var building in buildingManager.GetBuilding(areaPosition))
                {
                    if (_buildings.Contains(building.BuildingReference))
                    {
                        exited.Remove(building.BuildingReference);
                    }
                    else
                    {
                        _buildings.Add(building.BuildingReference);
                        onEntered(building);
                    }
                }
            }

            foreach (var structure in exited)
            {
                _buildings.Remove(structure);
            }
        }
    }
}