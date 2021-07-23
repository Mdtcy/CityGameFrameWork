using UnityEngine;
namespace Build.Map
{
    public interface IMap
    {
        bool       IsXY        { get; }
        Vector2Int Size        { get; }
        Vector3    CellOffset  { get; }
        Vector3    WorldCenter { get; }

        bool    IsInside(Vector2Int position);
        bool    IsWalkable(Vector2Int position);
        bool    IsBuildable(Vector2Int position, int mask);
        bool    CheckGround(Vector2Int position, Object[] options);
        Vector3 ClampPosition(Vector3 position);

        /// <summary>
        /// 获取一个值，避免同一个点的物体遮挡
        /// </summary>
        /// <returns></returns>
        Vector3 GetVariance();
    }
}