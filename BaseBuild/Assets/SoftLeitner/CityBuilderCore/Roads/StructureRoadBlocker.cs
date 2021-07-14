using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// blocker that uses the points of an <see cref="IStructure"/> on the same or the parent component
    /// </summary>
    public class StructureRoadBlocker : MonoBehaviour
    {
        public IStructure Structure { get; private set; }

        private void Start()
        {
            Structure = GetComponent<IStructure>() ?? GetComponentInParent<IStructure>();
            Structure.PositionsChanged += structurePositionsChanged;

            Dependencies.Get<IRoadManager>().Block(Structure.GetPoints());
        }

        private void structurePositionsChanged(PointsChanged<IStructure> change)
        {
            var roadManager = Dependencies.Get<IRoadManager>();

            roadManager.Unblock(change.RemovedPoints);
            roadManager.Block(change.AddedPoints);
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                Dependencies.Get<IRoadManager>().Unblock(Structure.GetPoints());
        }
    }
}