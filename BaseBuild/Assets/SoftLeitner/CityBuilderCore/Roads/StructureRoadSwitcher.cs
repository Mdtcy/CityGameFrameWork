using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// registers road switches at every point of the structure
    /// </summary>
    public class StructureRoadSwitcher : MonoBehaviour
    {
        public IStructure Structure { get; private set; }

        public Road RoadA;
        public Road RoadB;

        private void Start()
        {
            Structure = GetComponent<IStructure>() ?? GetComponentInParent<IStructure>();

            var roadManager = Dependencies.Get<IRoadManager>();

            foreach (var point in Structure.GetPoints())
            {
                roadManager.RegisterSwitch(point, RoadA, RoadB);
            }

            Structure.PositionsChanged += structurePositionsChanged;
        }

        private void structurePositionsChanged(PointsChanged<IStructure> change)
        {
            var roadManager = Dependencies.Get<IRoadManager>();

            roadManager.Deregister(change.RemovedPoints, RoadA);
            roadManager.Deregister(change.RemovedPoints, RoadB);

            foreach (var point in change.AddedPoints)
            {
                roadManager.RegisterSwitch(point, RoadA, RoadB);
            }
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
            {
                var roadManager = Dependencies.Get<IRoadManager>();
                var points = Structure.GetPoints();

                roadManager.Deregister(points, RoadA);
                roadManager.Deregister(points, RoadB);
            }
        }
    }
}
