using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// registers roads at every point of the structure
    /// </summary>
    public class StructureRoadRegisterer : MonoBehaviour
    {
        public IStructure Structure { get; private set; }

        public Road Road;

        private void Start()
        {
            Structure = GetComponent<IStructure>() ?? GetComponentInParent<IStructure>();

            Dependencies.Get<IRoadManager>().Register(Structure.GetPoints(), Road);
            
            Structure.PositionsChanged += structurePositionsChanged;
        }

        private void structurePositionsChanged(PointsChanged<IStructure> change)
        {
            var roadManager = Dependencies.Get<IRoadManager>();

            roadManager.Deregister(change.RemovedPoints, Road);
            roadManager.Register(change.AddedPoints, Road);
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
            {
                Dependencies.Get<IRoadManager>().Deregister(Structure.GetPoints(), Road);
            }
        }
    }
}
