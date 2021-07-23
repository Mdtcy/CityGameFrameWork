/**
 * @author BoLuo
 * @email [ tktetb@163.com ]
 * @create date 22:13:28
 * @modify date 22:13:28
 * @desc []
 */

using UnityEngine;

namespace Build.Map
{
    /// <summary>
    /// transforms between grid and world positions
    /// </summary>
    public interface IGridPositions
    {
        Vector2Int GetGridPosition(Vector3 position);
        Vector3    GetWorldPosition(Vector2Int point);
        Vector3    GetCenterFromPosition(Vector3 position);
        Vector3    GetPositionFromCenter(Vector3 center);
    }
}