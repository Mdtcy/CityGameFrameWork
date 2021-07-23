/**
 * @author BoLuo
 * @email [ tktetb@163.com ]
 * @create date 23:43:53
 * @modify date 23:43:53
 * @desc []
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Build.Structure
{
    /// <summary>
    /// interface for anything placed on the map(roads, decorators, buildings, ....)
    /// </summary>
    public interface IStructure
    {
        bool                    IsDestructible { get; }
        bool                    IsWalkable     { get; }
        IEnumerable<Vector2Int> GetPoints();
        bool                    HasPoint(Vector2Int point);
        void                    Remove(IEnumerable<Vector2Int> points);
    }
}