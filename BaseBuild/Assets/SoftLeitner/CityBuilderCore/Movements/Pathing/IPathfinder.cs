using UnityEngine;

namespace CityBuilderCore
{
    public interface IPathfinder
    {
        bool HasPoint(Vector2Int point, object tag = null);
        WalkingPath FindPath(Vector2Int start, Vector2Int target, object tag = null);
    }
    public interface IRoadPathfinder : IPathfinder { }
    public interface IRoadPathfinderBlocked : IPathfinder { }
    public interface IMapPathfinder : IPathfinder { }
    public interface IMapGridPathfinder : IPathfinder { }
}