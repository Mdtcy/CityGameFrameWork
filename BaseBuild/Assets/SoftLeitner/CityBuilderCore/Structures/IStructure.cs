using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// interface for anything placed on the map(roads, decorators, buildings, ....)
    /// </summary>
    public interface IStructure
    {
        /// <summary>
        /// reference to the structure that keeps working even if the structure is replaced
        /// </summary>
        StructureReference StructureReference { get; set; }

        bool IsDestructible { get; }
        bool IsDecorator { get; }
        bool IsWalkable { get; }

        int Level { get; }

        event Action<PointsChanged<IStructure>> PositionsChanged;

        string GetName();
        IEnumerable<Vector2Int> GetPoints();
        bool HasPoint(Vector2Int point);
        void Remove(IEnumerable<Vector2Int> points);
    }
}