
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Build.Map
{
    /// <summary>
    /// implementation for various map and grid functions on an isometric grid with 2d sprites
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class IsometricMap : MonoBehaviour, IMap, IGridPositions, IGridRotations
    {
        /// <summary>
        /// 地图大小
        /// </summary>
        public Vector2Int Size;

        //todo 待理解
        [Tooltip("tilemap containing the blocking tiles, if empty nothing is blocked")]
        public Tilemap Ground;

        //todo 待理解
        [Tooltip("tiles that are blocked in mapgrid pathfinding")]
        public TileBase[] WalkingBlockingTiles;

        //todo 待理解
        [Tooltip("tiles that are blocked for building")]
        public BlockingTile[] BuildingBlockingTiles;

        //todo 待理解
        [Tooltip("walkers will be randomly offset within variance so they dont overlap")]
        public float Variance;

        //todo 待理解
        public bool VerticalRotation;

        #region Properties

        public bool    IsXY        => Grid.cellSwizzle == GridLayout.CellSwizzle.XYZ;
        public Vector3 CellOffset  => Grid.cellSize + Grid.cellGap;
        public Vector3 WorldCenter => GetWorldPosition(Size / 2);

        Vector2Int IMap.Size => Size;

        public Grid Grid => _grid ? _grid : GetComponent<Grid>();

        #endregion

        // local
        private Grid _grid;

        protected virtual void Awake()
        {
            _grid = Grid;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(0, 0)), GetWorldPosition(new Vector2Int(Size.x, 0)));
            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(Size.x, 0)), GetWorldPosition(new Vector2Int(Size.x, Size.y)));
            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(Size.x, Size.y)), GetWorldPosition(new Vector2Int(0, Size.y)));
            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(0, Size.y)), GetWorldPosition(new Vector2Int(0, 0)));
        }

        #region IMap

        /// <summary>
        /// 坐标是否在Map内
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsInside(Vector2Int position)
        {
            if (position.x < 0 || position.y < 0)
                return false;
            if (position.x >= Size.x || position.y >= Size.y)
                return false;

            return true;
        }

        /// <summary>
        /// 是否可以建造
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public bool IsBuildable(Vector2Int position, int mask)
        {
            if (Ground)
                return !BuildingBlockingTiles.Where(b => b.Level.Check(mask))
                                             .Select(b => b.Tile)
                                             .Contains(Ground.GetTile((Vector3Int) position));
            else
                return true;
        }

        /// <summary>
        /// 是否可以行走
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsWalkable(Vector2Int position)
        {
            if (Ground)
                return !WalkingBlockingTiles.Contains(Ground.GetTile((Vector3Int) position));
            else
                return true;
        }

        /// <summary>
        /// Ground TileMap 在pos的位置有没有options里的Tile
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool CheckGround(Vector2Int position, Object[] options)
        {
            return options.Contains(Ground.GetTile((Vector3Int) position));
        }

        public Vector3 ClampPosition(Vector3 position)
        {
            if (IsXY)
                return new Vector3(Mathf.Clamp(position.x, -Size.x * CellOffset.x / 2f, Size.x * CellOffset.x / 2f),
                                   Mathf.Clamp(position.y, 0, Size.y * CellOffset.y), position.z);
            else
                return new Vector3(Mathf.Clamp(position.x, -Size.x * CellOffset.x / 2f, Size.x * CellOffset.x / 2f),
                                   position.y, Mathf.Clamp(position.z, 0, Size.y * CellOffset.y));
        }

        public Vector3 GetVariance()
        {
            if (Variance == 0f)
            {
                return Vector3.zero;
            }
            else
            {
                if (IsXY)
                    return new Vector3(Random.Range(-Variance, Variance), Random.Range(-Variance / 2f, Variance / 2f),
                                       0f);
                else
                    return new Vector3(Random.Range(-Variance, Variance), 0f,
                                       Random.Range(-Variance / 2f, Variance / 2f));
            }
        }

        #endregion

        #region IGridPosition

        public Vector2Int GetGridPosition(Vector3 position)
        {
            return (Vector2Int) Grid.WorldToCell(position);
        }

        public Vector3 GetWorldPosition(Vector2Int position)
        {
            return Grid.CellToWorld((Vector3Int) position);
        }

        public Vector3 GetCenterFromPosition(Vector3 position)
        {
            if (IsXY)
                return position + new Vector3(0, Grid.cellSize.y / 2f, 0);
            else
                return position + new Vector3(0, 0, Grid.cellSize.y / 2f);
        }

        public Vector3 GetPositionFromCenter(Vector3 center)
        {
            if (IsXY)
                return center - new Vector3(0, Grid.cellSize.y / 2f, 0);
            else
                return center - new Vector3(0, 0, Grid.cellSize.y / 2f);
        }

        #endregion

        #region IGridRotation

        public void SetRotation(Transform transform, Vector3 direction)
        {
            transform.localScale =
                new Vector3(direction.x > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
            if (VerticalRotation)
                transform.localRotation = Quaternion.Euler(direction.y > 0 ? 90f : 0f, 0, 0);
        }

        #endregion
    }
}