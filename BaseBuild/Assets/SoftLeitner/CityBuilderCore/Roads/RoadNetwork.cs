using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    public class RoadNetwork : IStructure, ILayerDependency
    {
        public event Action<PointsChanged<IStructure>> PositionsChanged;

        public Road Road { get; private set; }
        public Tilemap Tilemap { get; private set; }

        public GridPathfinding DefaultPathfinding { get; private set; }
        public GridPathfinding BlockedPathfinding { get; private set; }

        public StructureReference StructureReference { get; set; }

        public bool IsDestructible => Road?.IsDestructible ?? true;
        public bool IsDecorator => false;
        public bool IsWalkable => true;

        public int Level => Road == null ? _level : Road.Level.Value;

        private int _level;

        public RoadNetwork(Road road, Tilemap tilemap, int level = 0)
        {
            Tilemap = tilemap;
            Road = road;

            _level = level;

            DefaultPathfinding = new GridPathfinding();
            BlockedPathfinding = new GridPathfinding();

            StructureReference = new StructureReference(this);

            if (Tilemap != null)
            {
                foreach (var position in Tilemap.cellBounds.allPositionsWithin)
                {
                    if (Tilemap.HasTile(position))
                    {
                        DefaultPathfinding.Add((Vector2Int)position);
                        BlockedPathfinding.Add((Vector2Int)position);
                    }
                }
            }
        }

        public void Register()
        {
            Dependencies.Get<IStructureManager>().RegisterStructure(this, true);
        }

        public string GetName() => Road ? Road.Name : "Roads";

        public IEnumerable<Vector2Int> GetPoints() => DefaultPathfinding.GetPositions();
        public bool HasPoint(Vector2Int point) => DefaultPathfinding.HasPoint(point);

        public List<Vector2Int> Add(IEnumerable<Vector2Int> points, List<Vector2Int> blocked, Road road = null)
        {
            var structureManager = Dependencies.Get<IStructureManager>();

            List<Vector2Int> validPoints = points.Where(p => !DefaultPathfinding.HasPoint(p) && !structureManager.HasStructure(p, Level)).ToList();
            if (validPoints.Count == 0)
                return null;

            foreach (var point in validPoints)
            {
                Tilemap?.SetTile((Vector3Int)point, (road ?? Road).GetTile(point));
                DefaultPathfinding.Add(point);
                if (!blocked.Contains(point))
                    BlockedPathfinding.Add(point);
            }

            structureManager.Remove(validPoints, Level, true);

            return validPoints;
        }
        public void Remove(IEnumerable<Vector2Int> points)
        {
            foreach (var position in points)
            {
                Tilemap?.SetTile((Vector3Int)position, null);

                DefaultPathfinding.Remove(position);
                BlockedPathfinding.Remove(position);
            }

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, points, Enumerable.Empty<Vector2Int>()));
        }

        public void Register(IEnumerable<Vector2Int> points, List<Vector2Int> blocked)
        {
            foreach (var point in points)
            {
                DefaultPathfinding.Add(point);
                if (!blocked.Contains(point))
                    BlockedPathfinding.Add(point);
            }
        }
        public void Deregister(IEnumerable<Vector2Int> points)
        {
            foreach (var point in points.Where(p => !Tilemap || !Tilemap.HasTile((Vector3Int)p)))
            {
                DefaultPathfinding.Remove(point);
                BlockedPathfinding.Remove(point);
            }
        }

        public void RegisterSwitch(Vector2Int point, RoadNetwork other)
        {
            DefaultPathfinding.AddSwitch(point, other.DefaultPathfinding);
            BlockedPathfinding.AddSwitch(point, other.BlockedPathfinding);
        }
        public void RegisterSwitch(Vector2Int entry, Vector2Int point, Vector2Int exit, RoadNetwork other)
        {
            DefaultPathfinding.AddSwitch(entry, point, exit, other.DefaultPathfinding);
            BlockedPathfinding.AddSwitch(entry, point, exit, other.BlockedPathfinding);
        }


        public void Block(IEnumerable<Vector2Int> points)
        {
            foreach (var point in points)
            {
                if (DefaultPathfinding.HasPoint(point))
                    BlockedPathfinding.Remove(point);
            }
        }
        public void Unblock(IEnumerable<Vector2Int> points)
        {
            foreach (var point in points)
            {
                if (DefaultPathfinding.HasPoint(point))
                    BlockedPathfinding.Add(point);
            }
        }

        public void CheckLayers(IEnumerable<Vector2Int> points)
        {
            if (Tilemap == null)
                return;

            foreach (var position in points)
            {
                var currentTile = Tilemap.GetTile((Vector3Int)position);
                if (currentTile == null)
                    continue;

                var road = Dependencies.Get<IObjectSet<Road>>().Objects.FirstOrDefault(o => o.Stages.Any(s => s.Tile == currentTile));
                if (road == null)
                    continue;

                var roadTile = road.GetTile(position);
                if (currentTile == roadTile)
                    continue;

                Tilemap.SetTile((Vector3Int)position, roadTile);
            }
        }

        public bool TryGetRoad(Vector2Int point, out Road road, out string stage)
        {
            road = null;
            stage = null;

            if (!HasPoint(point))
                return false;

            var tile = Tilemap.GetTile((Vector3Int)point);
            if (tile == null)
                return false;

            foreach (var roadObject in Dependencies.Get<IObjectSet<Road>>().Objects)
            {
                foreach (var roadStage in roadObject.Stages)
                {
                    if (roadStage.Tile == tile)
                    {
                        road = roadObject;
                        stage = roadStage.Key;
                        return true;
                    }
                }
            }

            return false;
        }

        #region Saving
        [Serializable]
        public class RoadsData
        {
            public string Key;
            public RoadData[] Roads;
        }
        [Serializable]
        public class RoadData
        {
            public string Key;
            public Vector2Int[] Positions;
        }

        public RoadsData SaveData()
        {
            RoadsData data = new RoadsData() { Key = Road?.Key };

            Dictionary<TileBase, string> tiles = new Dictionary<TileBase, string>();
            foreach (var road in Dependencies.Get<IObjectSet<Road>>().Objects)
            {
                foreach (var stage in road.Stages)
                {
                    tiles.Add(stage.Tile, stage.Key);
                }
            }

            Dictionary<string, List<Vector2Int>> roadPositions = new Dictionary<string, List<Vector2Int>>();
            foreach (var position in Tilemap.cellBounds.allPositionsWithin)
            {
                if (Tilemap.HasTile(position))
                {
                    var tile = Tilemap.GetTile(position);
                    if (!tiles.ContainsKey(tile))
                        continue;
                    string key = tiles[tile];

                    if (!roadPositions.ContainsKey(key))
                        roadPositions.Add(key, new List<Vector2Int>());
                    roadPositions[key].Add((Vector2Int)position);
                }
            }

            data.Roads = roadPositions.Select(p => new RoadData() { Key = p.Key, Positions = p.Value.ToArray() }).ToArray();

            return data;
        }

        public void LoadData(RoadsData roadsData, List<Vector2Int> blocked)
        {
            var oldPoints = GetPoints();

            DefaultPathfinding.Clear();
            BlockedPathfinding.Clear();

            foreach (var point in oldPoints)
            {
                Tilemap.SetTile((Vector3Int)point, null);
            }

            Dictionary<string, TileBase> tiles = new Dictionary<string, TileBase>();
            foreach (var road in Dependencies.Get<IObjectSet<Road>>().Objects)
            {
                foreach (var stage in road.Stages)
                {
                    tiles.Add(stage.Key, stage.Tile);
                }
            }

            foreach (var data in roadsData.Roads)
            {
                if (!tiles.ContainsKey(data.Key))
                    continue;
                var tile = tiles[data.Key];

                foreach (var point in data.Positions)
                {
                    Tilemap.SetTile((Vector3Int)point, tile);

                    DefaultPathfinding.Add(point);
                    if (!blocked.Contains(point))
                        BlockedPathfinding.Add(point);
                }
            }

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, oldPoints, GetPoints()));
        }
        #endregion
    }
}
