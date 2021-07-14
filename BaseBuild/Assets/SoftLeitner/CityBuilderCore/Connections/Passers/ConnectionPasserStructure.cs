using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public class ConnectionPasserStructure : ConnectionPasserBase
    {
        private IStructure _structure;

        private void Awake()
        {
            _structure = GetComponent<IStructure>() ?? GetComponentInParent<IStructure>();
            _structure.PositionsChanged += structurePointsChanged;
        }

        public override IEnumerable<Vector2Int> GetPoints() => _structure.GetPoints();

        private void structurePointsChanged(PointsChanged<IStructure> change) => onPointsChanged(change.RemovedPoints, change.AddedPoints);
    }
}
