using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    /// <summary>
    /// implementation for various map and grid functions on an isometric grid with 2d sprites
    /// </summary>
    [RequireComponent(typeof(Grid))]
    public class IsometricMap : MonoBehaviour, IMap, IGridPositions, IGridRotations
    {
        public Vector2Int Size;
        [Tooltip("tilemap containing the blocking tiles, if empty nothing is blocked")]
        public Tilemap Ground;
        [Tooltip("tiles that are blocked in mapgrid pathfinding")]
        public TileBase[] WalkingBlockingTiles;
        [Tooltip("tiles that are blocked for building")]
        public BlockingTile[] BuildingBlockingTiles;
        [Tooltip("walkers will be randomly offset within variance so they dont overlap")]
        public float Variance;
        public bool VerticalRotation;

        public bool IsXY => Grid.cellSwizzle == GridLayout.CellSwizzle.XYZ;
        public Vector3 CellOffset => Grid.cellSize + Grid.cellGap;
        public Vector3 WorldCenter => GetWorldPosition(Size / 2);

        Vector2Int IMap.Size => Size;

        public Grid Grid => _grid ? _grid : GetComponent<Grid>();

        private Grid _grid;

        protected virtual void Awake()
        {
            _grid = Grid;

            Dependencies.Register<IMap>(this);
            Dependencies.Register<IGridPositions>(this);
            Dependencies.Register<IGridRotations>(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(0, 0)), GetWorldPosition(new Vector2Int(Size.x, 0)));
            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(Size.x, 0)), GetWorldPosition(new Vector2Int(Size.x, Size.y)));
            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(Size.x, Size.y)), GetWorldPosition(new Vector2Int(0, Size.y)));
            Gizmos.DrawLine(GetWorldPosition(new Vector2Int(0, Size.y)), GetWorldPosition(new Vector2Int(0, 0)));
        }

        public bool IsInside(Vector2Int position)
        {
            if (position.x < 0 || position.y < 0)
                return false;
            if (position.x >= Size.x || position.y >= Size.y)
                return false;
            return true;
        }

        public bool IsBuildable(Vector2Int position, int mask)
        {
            if (Ground)
                return !BuildingBlockingTiles.Where(b => b.Level.Check(mask)).Select(b => b.Tile).Contains(Ground.GetTile((Vector3Int)position));
            else
                return true;
        }

        public bool IsWalkable(Vector2Int position)
        {
            if (Ground)
                return !WalkingBlockingTiles.Contains(Ground.GetTile((Vector3Int)position));
            else
                return true;
        }

        public bool CheckGround(Vector2Int position, Object[] options)
        {
            return options.Contains(Ground.GetTile((Vector3Int)position));
        }

        public Vector3 ClampPosition(Vector3 position)
        {
            if (IsXY)
                return new Vector3(Mathf.Clamp(position.x, -Size.x * CellOffset.x / 2f, Size.x * CellOffset.x / 2f), Mathf.Clamp(position.y, 0, Size.y * CellOffset.y), position.z);
            else
                return new Vector3(Mathf.Clamp(position.x, -Size.x * CellOffset.x / 2f, Size.x * CellOffset.x / 2f), position.y, Mathf.Clamp(position.z, 0, Size.y * CellOffset.y));
        }

        public Vector2Int GetGridPosition(Vector3 position)
        {
            return (Vector2Int)Grid.WorldToCell(position);
        }

        public Vector3 GetWorldPosition(Vector2Int position)
        {
            return Grid.CellToWorld((Vector3Int)position);
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

        public Vector3 GetVariance()
        {
            if (Variance == 0f)
            {
                return Vector3.zero;
            }
            else
            {
                if (IsXY)
                    return new Vector3(Random.Range(-Variance, Variance), Random.Range(-Variance / 2f, Variance / 2f), 0f);
                else
                    return new Vector3(Random.Range(-Variance, Variance), 0f, Random.Range(-Variance / 2f, Variance / 2f));
            }
        }

        public void SetRotation(Transform transform, Vector3 direction)
        {
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
            if (VerticalRotation)
                transform.localRotation = Quaternion.Euler(direction.y > 0 ? 90f : 0f, 0, 0);
        }
    }
}