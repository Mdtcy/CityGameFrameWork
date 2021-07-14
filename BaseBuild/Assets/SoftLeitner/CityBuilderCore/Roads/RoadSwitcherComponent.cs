using UnityEngine;

namespace CityBuilderCore
{
    public class RoadSwitcherComponent : BuildingComponent
    {
        public override string Key => "ROS";

        public Vector2Int EntryPoint;
        public Vector2Int SwitchPoint;
        public Vector2Int ExitPoint;

        public Road EntryRoad;
        public Road ExitRoad;

        private void Start()
        {
            Dependencies.Get<IRoadManager>().RegisterSwitch(Building.RotateBuildingPoint(EntryPoint), Building.RotateBuildingPoint(SwitchPoint), Building.RotateBuildingPoint(ExitPoint), EntryRoad, ExitRoad);
        }
        private void OnDestroy()
        {
            if (!gameObject.scene.isLoaded)
                return;

            Dependencies.Get<IRoadManager>().Deregister(new[] { Building.RotateBuildingPoint(SwitchPoint) }, EntryRoad);
            Dependencies.Get<IRoadManager>().Deregister(new[] { Building.RotateBuildingPoint(SwitchPoint) }, ExitRoad);
        }
    }
}
