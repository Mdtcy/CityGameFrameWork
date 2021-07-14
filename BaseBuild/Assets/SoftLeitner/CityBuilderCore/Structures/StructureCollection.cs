using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// structure made up of a collection of gameobjects
    /// </summary>
    public class StructureCollection : MonoBehaviour, IStructure
    {
        public string Key;
        public string Name;

        public bool IsDestructible = true;
        public bool IsDecorator = false;
        public bool IsWalkable = false;

        [Tooltip("determines which other structures can reside in the same points")]
        public StructureLevelMask Level;

        public GameObject Prefab;
        public Vector2Int ObjectSize = Vector2Int.one;

        bool IStructure.IsDestructible => IsDestructible;
        bool IStructure.IsDecorator => IsDecorator;
        bool IStructure.IsWalkable => IsWalkable;
        int IStructure.Level => Level.Value;

        public StructureReference StructureReference { get; set; }

        public bool IsReplica { get; set; }

        public Transform Root => transform;

        public event Action<PointsChanged<IStructure>> PositionsChanged;

        private Dictionary<Vector2Int, GameObject> _objects = new Dictionary<Vector2Int, GameObject>();

        private void Start()
        {
            var positions = Dependencies.Get<IGridPositions>();
            foreach (Transform child in transform)
            {
                var position = positions.GetGridPosition(child.position);

                for (int x = 0; x < ObjectSize.x; x++)
                {
                    for (int y = 0; y < ObjectSize.y; y++)
                    {
                        _objects.Add(position + new Vector2Int(x, y), child.gameObject);
                    }
                }
            }

            StructureReference = new StructureReference(this);

            if (!IsReplica)
                Dependencies.Get<IStructureManager>().RegisterStructure(this);

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), GetPoints()));
        }

        private void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                Dependencies.Get<IStructureManager>().DeregisterStructure(this);
        }

        public IEnumerable<Vector2Int> GetPoints() => _objects.Keys;

        public bool HasPoint(Vector2Int position) => _objects.ContainsKey(position);

        public void Add(Vector2Int position) => Add(new Vector2Int[] { position });
        public void Add(IEnumerable<Vector2Int> positions)
        {
            foreach (var position in positions)
            {
                var instance = Instantiate(Prefab, transform);
                instance.transform.position = Dependencies.Get<IGridPositions>().GetWorldPosition(position);
                _objects.Add(position, instance);
            }

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, Enumerable.Empty<Vector2Int>(), positions));
        }
        public void Remove(Vector2Int position) => Remove(new Vector2Int[] { position });
        public void Remove(IEnumerable<Vector2Int> positions)
        {
            List<GameObject> children = new List<GameObject>();
            foreach (var position in positions)
            {
                if (_objects.ContainsKey(position) && !children.Contains(_objects[position]))
                    children.Add(_objects[position]);
            }

            foreach (var child in children)
            {
                var position = Dependencies.Get<IGridPositions>().GetGridPosition(child.transform.position);

                for (int x = 0; x < ObjectSize.x; x++)
                {
                    for (int y = 0; y < ObjectSize.y; y++)
                    {
                        _objects.Remove(position + new Vector2Int(x, y));
                    }
                }

                Destroy(child);
            }

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, positions, Enumerable.Empty<Vector2Int>()));
        }

        public void Clear()
        {
            _objects.ForEach(o => Destroy(o.Value));
            _objects.Clear();
        }

        public string GetName() => Name;
        
        #region Saving
        [Serializable]
        public class StructureCollectionData
        {
            public string Key;
            public Vector2Int[] Positions;
            public string[] InstanceData;
        }

        public StructureCollectionData SaveData()
        {
            var data = new StructureCollectionData();

            data.Key = Key;
            data.Positions = _objects.Keys.ToArray();

            if (Prefab.GetComponent<ISaveData>() != null)
                data.InstanceData = _objects.Values.Select(o => o.GetComponent<ISaveData>().SaveData()).ToArray();

            return data;
        }
        public void LoadData(StructureCollectionData data)
        {
            var oldPoints = _objects.Keys.ToList();

            Clear();

            foreach (var position in data.Positions)
            {
                var instance = Instantiate(Prefab, transform);
                instance.transform.position = Dependencies.Get<IGridPositions>().GetWorldPosition(position);
                _objects.Add(position, instance);
            }

            if (data.InstanceData != null && data.InstanceData.Length == _objects.Count)
            {
                for (int i = 0; i < data.InstanceData.Length; i++)
                {
                    _objects.ElementAt(i).Value.GetComponent<ISaveData>().LoadData(data.InstanceData[i]);
                }
            }

            PositionsChanged?.Invoke(new PointsChanged<IStructure>(this, oldPoints, GetPoints()));
        }
        #endregion
    }
}