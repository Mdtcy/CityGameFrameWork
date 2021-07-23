using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Build.Map
{
    /// <summary>
    /// default implementation for various map and grid functions<br/>
    /// meant for rectangle grid and 3d display
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class DefaultMap : MonoBehaviour, IMap, IGridOverlay, IGridPositions, IGridRotations
    {
        public Vector2Int Size;
        [Tooltip("tilemap containing the blocking tiles, if empty nothing is blocked")]
        public Tilemap Ground;
        [Tooltip("tiles that are blocked in mapgrid pathfinding")]
        public TileBase[] WalkingBlockingTiles;
        [Tooltip("tiles that are blocked for building")]
        public BlockingTile[] BuildingBlockingTiles;
        [Tooltip("visual shown as an overlay when building")]
        public MeshRenderer GridVisual;
        [Tooltip("walkers will be randomly offset within variance so they dont overlap")]
        public float Variance;

        public bool IsXY => Grid.cellSwizzle == GridLayout.CellSwizzle.XYZ;
        public Vector3 CellOffset => Grid.cellSize + Grid.cellGap;
        public Vector3 WorldCenter => IsXY ? new Vector3(Size.x / 2f * CellOffset.x, Size.y / 2f * CellOffset.y, 0) : new Vector3(Size.x / 2f * CellOffset.x, 0, Size.y / 2f * CellOffset.y);
        public Vector3 WorldSize => IsXY ? new Vector3(Size.x * CellOffset.x, Size.y * CellOffset.y, 1) : new Vector3(Size.x * CellOffset.x, 1, Size.y * CellOffset.y);

        Vector2Int IMap.Size => Size;

        public Grid Grid => _grid ? _grid : GetComponent<Grid>();


        private Grid _grid;

        protected virtual void Awake()
        {
            _grid = Grid;

            // Dependencies.Register<IMap>(this);
            // Dependencies.Register<IGridOverlay>(this);
            // Dependencies.Register<IGridPositions>(this);
            // Dependencies.Register<IGridRotations>(this);
        }

        protected virtual void Start()
        {
            if (GridVisual)
            {
                GridVisual.transform.position = WorldCenter;
                GridVisual.transform.localScale = WorldSize / 10f;
                GridVisual.material.mainTextureScale = new Vector2(Size.x, Size.y);
                GridVisual.gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(WorldCenter, WorldSize);
        }

        #region IMap

        /// <summary>
        /// position是否在图内
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
        /// 如果pos位置的Tile在BuildingBlockingTiles中并且要建造的mask和blockingTile的层次相同，则不可建造
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
        /// （有Ground）position所在位置的Tile是否被包含在WalkingBlockingTiles中
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
        /// options中是否包括position位置的Tile
        /// </summary>
        /// <param name="position"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool CheckGround(Vector2Int position, Object[] options)
        {
            return options.Contains(Ground.GetTile((Vector3Int) position));
        }

        /// <summary>
        /// 返回一个在这张图内的Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 ClampPosition(Vector3 position)
        {
            if (IsXY)
                return new Vector3(Mathf.Clamp(position.x, 0, Size.x * CellOffset.x),
                                   Mathf.Clamp(position.y, 0, Size.y * CellOffset.y), position.z);
            else
                return new Vector3(Mathf.Clamp(position.x, 0, Size.x * CellOffset.x), position.y,
                                   Mathf.Clamp(position.z, 0, Size.y * CellOffset.y));
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
                    return new Vector3(Random.Range(-Variance, Variance), Random.Range(-Variance, Variance), 0f);
                else
                    return new Vector3(Random.Range(-Variance, Variance), 0f, Random.Range(-Variance, Variance));
            }
        }

        #endregion


        #region IGridOverlay

        public void Show() => setGridVisibility(true);
        public void Hide() => setGridVisibility(false);

        private void setGridVisibility(bool visible)
        {
            if (GridVisual)
                GridVisual.gameObject.SetActive(visible);
        }

        #endregion

        #region IGridPositions

        public Vector2Int GetGridPosition(Vector3 position)
        {
            return (Vector2Int) Grid.WorldToCell(position);
        }

        public Vector3 GetWorldPosition(Vector2Int position)
        {
            return Grid.CellToWorld((Vector3Int) position);
        }

        /// <summary>
        /// 输入世界坐标，返回对应Cell的中心
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 GetCenterFromPosition(Vector3 position)
        {
            if (IsXY)
                return position + new Vector3(Grid.cellSize.x / 2f, Grid.cellSize.y / 2f, 0f);
            else
                return position + new Vector3(Grid.cellSize.x / 2f, 0f, Grid.cellSize.y / 2f);
        }

        /// <summary>
        /// 输入对应Cell的中心，返回对应的世界坐标
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 GetPositionFromCenter(Vector3 center)
        {
            return center - new Vector3(Grid.cellSize.x / 2f, 0f, Grid.cellSize.y / 2f);
        }

        #endregion

        #region IGridRotations

        public void SetRotation(Transform transform, Vector3 direction)
        {
            if (transform)
            {
                if (IsXY)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
                else
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }

        #endregion
    }
}