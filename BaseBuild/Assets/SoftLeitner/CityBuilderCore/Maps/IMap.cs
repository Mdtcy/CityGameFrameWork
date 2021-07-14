using UnityEngine;

namespace CityBuilderCore
{
    public interface IMap
    {
        bool IsXY { get; }
        Vector2Int Size { get; }
        Vector3 CellOffset { get; }
        Vector3 WorldCenter { get; }

        bool IsInside(Vector2Int position);
        bool IsWalkable(Vector2Int position);
        bool IsBuildable(Vector2Int position, int mask);
        bool CheckGround(Vector2Int position, Object[] options);
        Vector3 ClampPosition(Vector3 position);
        /// <summary>
        /// get position variance for walkers and such, can be used so walkers on the same point dont overlap
        /// </summary>
        /// <returns></returns>
        Vector3 GetVariance();
    }
}