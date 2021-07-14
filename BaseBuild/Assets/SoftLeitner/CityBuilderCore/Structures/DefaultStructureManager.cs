using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace CityBuilderCore
{
    public class DefaultStructureManager : MonoBehaviour, IStructureManager, IMapPathfinder, IMapGridPathfinder
    {
        public const int LEVEL_COUNT = 8;

        private readonly Dictionary<int, StructureLevelManager> _levels = new Dictionary<int, StructureLevelManager>();

        private readonly List<StructureCollection> _collections = new List<StructureCollection>();
        private readonly List<StructureTiles> _tiles = new List<StructureTiles>();
        private readonly List<StructureReference> _underlying = new List<StructureReference>();

        private bool _isInitialized;
        private IMap _map;

        private GridPathfinding _gridPathfinding = new GridPathfinding();

        protected virtual void Awake()
        {
            Dependencies.Register<IStructureManager>(this);
            Dependencies.Register<IMapPathfinder>(this);
            Dependencies.Register<IMapGridPathfinder>(_gridPathfinding);
        }

        protected virtual void Start()
        {
            initialize();
        }

        private void initialize()
        {
            if (_isInitialized)
                return;

            _map = Dependencies.Get<IMap>();

            for (int x = 0; x < _map.Size.x; x++)
            {
                for (int y = 0; y < _map.Size.y; y++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    if (_map.IsWalkable(position))
                        _gridPathfinding.Add(position);
                }
            }

            _isInitialized = true;
        }

        public bool CheckAvailability(Vector2Int point, int mask)
        {
            if (!_map.IsBuildable(point, mask))
                return false;
            if (!_map.IsInside(point))
                return false;
            if (HasStructure(point, mask))
                return false;
            return true;
        }

        public bool HasStructure(Vector2Int position, int mask) => GetStructures(position, mask).Any();
        public bool HasStructure(Vector2Int position, int mask, bool? isWalkable) => GetStructures(position, mask, isWalkable).Any();

        public IEnumerable<IStructure> GetStructures(Vector2Int position, int mask) => GetStructures(position, mask, null);
        public IEnumerable<IStructure> GetStructures(Vector2Int position, int mask, bool? isWalkable)
        {
            List<IStructure> levelStructures = new List<IStructure>();
            foreach (var level in _levels)
            {
                if (!StructureLevelMask.Check(mask, level.Key))
                    continue;

                var structure = level.Value.GetStructure(position);
                if (structure == null)
                    continue;

                if (isWalkable.HasValue && structure.IsWalkable != isWalkable.Value)
                    continue;

                if (!levelStructures.Contains(structure))
                {
                    levelStructures.Add(structure);
                    yield return structure;
                }
            }

            foreach (var collection in _collections.Where(c => c.Level.Check(mask) && c.HasPoint(position) && (!isWalkable.HasValue || c.IsWalkable == isWalkable.Value)))
            {
                yield return collection;
            }

            foreach (var tiles in _tiles.Where(c => c.Level.Check(mask) && c.HasPoint(position) && (!isWalkable.HasValue || c.IsWalkable == isWalkable.Value)))
            {
                yield return tiles;
            }

            foreach (var underlying in _underlying.Select(u => u.Structure).Where(c => StructureLevelMask.Check(c.Level, mask) && c.HasPoint(position) && (!isWalkable.HasValue || c.IsWalkable == isWalkable.Value)))
            {
                yield return underlying;
            }
        }
        public StructureCollection GetStructureCollection(string key) => _collections.FirstOrDefault(c => c.Key == key);
        public StructureTiles GetStructureTiles(string key) => _tiles.FirstOrDefault(c => c.Key == key);

        public int RemoveDecorators(IEnumerable<Vector2Int> positions, int mask) => Remove(positions, mask, true);
        public int Remove(IEnumerable<Vector2Int> positions, int mask, bool decoratorsOnly, Action<IStructure> removing = null)
        {
            List<StructureReference> structures = new List<StructureReference>();

            foreach (var position in positions)
            {
                foreach (var structure in GetStructures(position, mask))
                {
                    if (!structures.Contains(structure.StructureReference))
                        structures.Add(structure.StructureReference);
                }
            }

            foreach (var structure in structures)
            {
                if (!structure.Structure.IsDestructible)
                    continue;
                if (decoratorsOnly && !structure.Structure.IsDecorator)
                    continue;

                removing?.Invoke(structure.Structure);

                structure.Structure.Remove(positions);
            }

            return structures.Count;
        }

        public void RegisterStructure(IStructure structure, bool isUnderlying = false)
        {
            initialize();

            if (structure is StructureCollection collection)
            {
                structure.PositionsChanged += structurePositionsChanged;
                _collections.Add(collection);
            }
            else if (structure is StructureTiles tiles)
            {
                structure.PositionsChanged += structurePositionsChanged;
                _tiles.Add(tiles);
            }
            else if (isUnderlying)
            {
                structure.PositionsChanged += structurePositionsChanged;
                _underlying.Add(structure.StructureReference);
            }
            else
            {
                foreach (var manager in getManagers(structure.Level))
                {
                    manager.AddStructure(structure);
                }
            }

            checkPoints(structure.GetPoints());
        }
        public void DeregisterStructure(IStructure structure, bool isUnderlying = false)
        {
            if (structure is StructureCollection collection)
            {
                structure.PositionsChanged -= structurePositionsChanged;
                _collections.Remove(collection);
            }
            else if (structure is StructureTiles tiles)
            {
                structure.PositionsChanged -= structurePositionsChanged;
                _tiles.Remove(tiles);
            }
            else if (isUnderlying)
            {
                structure.PositionsChanged -= structurePositionsChanged;
                _underlying.Remove(structure.StructureReference);
            }
            else
            {
                foreach (var manager in getManagers(structure.Level))
                {
                    manager.RemoveStructure(structure);
                }
            }

            checkPoints(structure.GetPoints());
        }

        public bool HasPoint(Vector2Int point, object tag = null)
        {
            return NavMesh.SamplePosition(Dependencies.Get<IGridPositions>().GetWorldPosition(point), out _, 1f, NavMesh.AllAreas);
        }
        public WalkingPath FindPath(Vector2Int startPosition, Vector2Int targetPosition, object tag = null)
        {
            var positions = Dependencies.Get<IGridPositions>();
            var path = new NavMeshPath();

            NavMesh.CalculatePath(positions.GetCenterFromPosition(positions.GetWorldPosition(startPosition)), positions.GetCenterFromPosition(positions.GetWorldPosition(targetPosition)), NavMesh.AllAreas, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return new WalkingPath(path.corners.Select(c => positions.GetPositionFromCenter(c)).ToArray());
            }

            return new WalkingPath(new [] { startPosition, targetPosition });
        }

        private IEnumerable<StructureLevelManager> getManagers(int level)
        {
            if (level == 0)
            {
                if (!_levels.ContainsKey(0))
                    _levels.Add(0, new StructureLevelManager());
                yield return _levels[0];
            }
            else
            {
                int pow = 1;
                for (int i = 0; i < LEVEL_COUNT; i++)
                {
                    if ((level & pow) == pow)
                    {
                        if (!_levels.ContainsKey(pow))
                            _levels.Add(pow, new StructureLevelManager());
                        yield return _levels[pow];
                    }
                    pow *= 2;
                }
            }
        }

        private void structurePositionsChanged(PointsChanged<IStructure> change)
        {
            checkPoints(change.AddedPoints);
            checkPoints(change.RemovedPoints);
        }

        private void checkPoints(IEnumerable<Vector2Int> points)
        {
            foreach (var point in points)
            {
                if (_map.IsWalkable(point))
                {
                    if (HasStructure(point, 0, false))
                        _gridPathfinding.Remove(point);
                    else
                        _gridPathfinding.Add(point);
                }
                else
                {
                    if (HasStructure(point, 0, true))
                        _gridPathfinding.Add(point);
                    else
                        _gridPathfinding.Remove(point);
                }
            }
        }

        #region Saving
        [Serializable]
        public class StructuresData
        {
            public StructureCollection.StructureCollectionData[] Collections;
            public StructureTiles.StructureTilesData[] Tiles;
        }

        public string SaveData()
        {
            return JsonUtility.ToJson(new StructuresData()
            {
                Collections = _collections.Where(c => c.IsDestructible).Select(c => c.SaveData()).ToArray(),
                Tiles = _tiles.Where(t => t.IsDestructible).Select(t => t.SaveData()).ToArray(),
            });
        }
        public void LoadData(string json)
        {
            var structuresData = JsonUtility.FromJson<StructuresData>(json);

            if (structuresData.Collections != null)
            {
                foreach (var data in structuresData.Collections)
                {
                    var collection = _collections.FirstOrDefault(c => c.Key == data.Key);
                    if (collection == null)
                        continue;

                    collection.LoadData(data);
                }
            }

            if (structuresData.Tiles != null)
            {
                foreach (var data in structuresData.Tiles)
                {
                    var tiles = _tiles.FirstOrDefault(c => c.Key == data.Key);
                    if (tiles == null)
                        continue;

                    tiles.LoadData(data);
                }
            }
        }
        #endregion
    }
}