using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// fallback pathfinding that just returns a straight path from start to target
    /// </summary>
    public class NoPathfinding : IPathfinder
    {
        public WalkingPath FindPath(Vector2Int start, Vector2Int target, object parameter = null)
        {
            return new WalkingPath(new [] { start, target });
        }

        public bool HasPoint(Vector2Int point, object tag = null)
        {
            return true;
        }
    }
}