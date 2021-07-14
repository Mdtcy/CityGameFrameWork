using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// transforms between grid and world positions
    /// </summary>
    public interface IGridPositions
    {
        Vector2Int GetGridPosition(Vector3 position);
        Vector3 GetWorldPosition(Vector2Int point);
        Vector3 GetCenterFromPosition(Vector3 position);
        Vector3 GetPositionFromCenter(Vector3 center);
    }
}