using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// helper that resolves pathtype into a pathfinder provides some convenience functions
    /// </summary>
    public static class PathHelper
    {
        private static NoPathfinding _noPathfinding = new NoPathfinding();

        public static bool HasAdjacent(Vector2Int point, PathType type, object tag = null)
        {
            foreach (var adjacent in PositionHelper.GetAreaCrossPositions(point, 1))
            {
                if (CheckPoint(adjacent, type, tag))
                    return true;
            }
            return false;
        }
        public static IEnumerable<Vector2Int> GetAdjacent(Vector2Int point, PathType type, object tag = null)
        {
            foreach (var adjacent in PositionHelper.GetAreaCrossPositions(point, 1))
            {
                if (CheckPoint(adjacent, type, tag))
                    yield return adjacent;
            }
        }

        public static bool CheckPoint(Vector2Int point, PathType type, object tag = null)
        {
            return getPathfinder(type).HasPoint(point, tag);
        }

        public static WalkingPath FindPath(IBuilding start, Vector2Int? current, IBuilding target, PathType type, object tag = null)
        {
            if (current.HasValue)
                return FindPath(current.Value, target, type, tag);
            else
                return FindPath(start, target, type, tag);
        }
        public static WalkingPath FindPath(Vector2Int start, Vector2Int target, PathType type, object tag = null)
        {
            return getPathfinder(type).FindPath(start, target, tag);
        }
        public static WalkingPath FindPath(Vector2Int start, IBuilding target, PathType type, object tag = null)
        {
            var targetAccess = target.GetAccessPoint(type, tag);
            if (targetAccess.HasValue)
                return getPathfinder(type).FindPath(start, targetAccess.Value, tag);
            else
                return null;
        }
        public static WalkingPath FindPath(IBuilding start, IBuilding target, PathType type, object tag = null)
        {
            var startAccess = start.GetAccessPoint(type, tag);
            var targetAccess = target.GetAccessPoint(type, tag);

            if (startAccess.HasValue && targetAccess.HasValue)
                return getPathfinder(type).FindPath(startAccess.Value, targetAccess.Value, tag);
            else
                return null;
        }
        public static WalkingPath FindPath(IBuilding start, Vector2Int target, PathType type, object tag = null)
        {
            var startAccess = start.GetAccessPoint(type, tag);
            if (startAccess.HasValue)
                return getPathfinder(type).FindPath(startAccess.Value, target, tag);
            else
                return null;
        }

        private static IPathfinder getPathfinder(PathType type)
        {
            switch (type)
            {
                case PathType.Any:
                    return Dependencies.Contains<IPathfinder>() ? Dependencies.Get<IPathfinder>() : Dependencies.Get<IRoadPathfinder>();
                case PathType.Road:
                    return Dependencies.Get<IRoadPathfinder>();
                case PathType.RoadBlocked:
                    return Dependencies.Get<IRoadPathfinderBlocked>();
                case PathType.Map:
                    return Dependencies.Get<IMapPathfinder>();
                case PathType.MapGrid:
                    return Dependencies.Get<IMapGridPathfinder>();
                default:
                    return _noPathfinding;
            }
        }
    }
}