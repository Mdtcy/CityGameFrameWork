using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    /// <summary>
    /// structure made up of a collection of tiles
    /// </summary>
    public class StructureTiles : MonoBehaviour, IStructure
    {
        public string Key;
        public string Name;

        public bool IsDestructible;
        public bool IsDecorator;
        public bool IsWalkable;

        [Tooltip("determines which other structures can reside in the same points")]
        public StructureLevelMask Level;

        public Tilemap Tilemap;
        public TileBase Tile;

        public StructureCollection ReplicaCollection;

        public StructureReference StructureReference { get; set; }

        public Transform Root => transform;
        public bool Changed { get; private set; }

        bool IStructure.IsDestructible => IsDestructible;
        bool IStructure.IsDecorator => IsDecorator;
        bool IStructure.IsWalkable => IsWalkable;
        int IStructure.Level => Level.Value;

        public event Action<PointsChanged<IStructure>> PositionsChanged;

        private List<Vector2Int> _positions = new List<Vector2Int>();

        private void Awake()
        {
            if (ReplicaCollection)
                ReplicaCollection.IsReplica = true;
        }

        private void Start()
        {
            foreach (var position in Tilemap.cellBounds.allPositionsWithin)
            {
                if (Tile == null && Tilemap.HasTile(position) || Tilemap.GetTile(position) == Tile)
                {
                    _positions.Add((Vector2Int)position);
                }
            }

            if (ReplicaCollection)
                ReplicaCollection.Add(_positions);

            StructureReference = new StructureReference(this);

            Dependencies.Get<IStructureManager>().RegisterStructure(this);

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), _positions));
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                Dependencies.Get<IStructureManager>().DeregisterStructure(this);
        }

        public IEnumerable<Vector2Int> GetPoints() => _positions;

        public bool HasPoint(Vector2Int position) => _positions.Contains(position);

        public void Add(Vector2Int position) => Add(new Vector2Int[] { position });
        public void Add(IEnumerable<Vector2Int> positions)
        {
            foreach (var position in positions)
            {
                _positions.Add(position);
                Tilemap.SetTile((Vector3Int)position, Tile);
            }

            if (ReplicaCollection)
                ReplicaCollection.Add(positions);

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), positions));
        }
        public void Remove(Vector2Int position) => Remove(new Vector2Int[] { position });
        public void Remove(IEnumerable<Vector2Int> positions)
        {
            foreach (var position in positions)
            {
                _positions.Remove(position);
                Tilemap.SetTile((Vector3Int)position, null);
            }

            if (ReplicaCollection)
                ReplicaCollection.Remove(positions);

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, positions, Enumerable.Empty<Vector2Int>()));
        }

        public void RefreshTile(Vector2Int position)
        {
            Tilemap.RefreshTile((Vector3Int)position);
        }

        public string GetName() => Name;

        #region Saving
        [Serializable]
        public class StructureTilesData
        {
            public string Key;
            public Vector2Int[] Positions;
        }

        public StructureTilesData SaveData()
        {
            return new StructureTilesData()
            {
                Key = Key,
                Positions = _positions.ToArray()
            };
        }
        public void LoadData(StructureTilesData data)
        {
            var oldPositions = _positions;

            _positions.ForEach(p => Tilemap.SetTile((Vector3Int)p, null));
            _positions = new List<Vector2Int>();

            if (ReplicaCollection)
                ReplicaCollection.Clear();

            foreach (var position in data.Positions)
            {
                Tilemap.SetTile((Vector3Int)position, Tile);
                _positions.Add(position);
            }

            if (ReplicaCollection)
                ReplicaCollection.Add(data.Positions);

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, oldPositions, _positions));
        }
        #endregion
    }
}