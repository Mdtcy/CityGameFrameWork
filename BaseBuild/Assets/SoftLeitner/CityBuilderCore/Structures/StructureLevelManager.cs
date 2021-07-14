using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public class StructureLevelManager
    {
        private readonly Dictionary<Vector2Int, StructureReference> _structurePositions = new Dictionary<Vector2Int, StructureReference>();

        public void AddStructure(IStructure structure)
        {
            foreach (var structurePosition in structure.GetPoints())
            {
                _structurePositions.Add(structurePosition, structure.StructureReference);
            }
        }
        public void RemoveStructure(IStructure structure)
        {
            foreach (var structurePosition in structure.GetPoints())
            {
                if (_structurePositions.ContainsKey(structurePosition) && _structurePositions[structurePosition] == structure.StructureReference)
                    _structurePositions.Remove(structurePosition);
            }
        }

        public IStructure GetStructure(Vector2Int position)
        {
            if (_structurePositions.ContainsKey(position))
                return _structurePositions[position].Structure;
            return null;
        }
    }
}
