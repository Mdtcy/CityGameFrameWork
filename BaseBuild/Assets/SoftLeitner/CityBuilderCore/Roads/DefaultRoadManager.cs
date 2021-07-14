using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    [RequireComponent(typeof(Tilemap))]
    public class DefaultRoadManager : MonoBehaviour, IRoadManager
    {
        [Tooltip("determines which structures can reside in the same points as roads")]
        public StructureLevelMask Level;

        public Transform Root => transform;

        public StructureReference StructureReference { get; set; }

        private RoadNetwork _network;
        private List<Vector2Int> _blocked;

        protected virtual void Awake()
        {
            _network = new RoadNetwork(null, GetComponent<Tilemap>(), Level.Value);
            _blocked = new List<Vector2Int>();

            Dependencies.Register<IRoadManager>(this);
            Dependencies.Register<IRoadPathfinder>(_network.DefaultPathfinding);
            Dependencies.Register<IRoadPathfinderBlocked>(_network.BlockedPathfinding);
        }

        protected virtual void Start()
        {
            _network.Register();
        }

        public bool HasPoint(Vector2Int point, Road road = null) => _network.DefaultPathfinding.HasPoint(point, road);

        public void Add(IEnumerable<Vector2Int> positions, Road road) => _network.Add(positions, _blocked, road);

        public void Register(IEnumerable<Vector2Int> points, Road road) => _network.Register(points, _blocked);
        public void Deregister(IEnumerable<Vector2Int> points, Road road) => _network.Deregister(points);

        public void RegisterSwitch(Vector2Int point, Road roadA, Road roadB) => throw new System.NotImplementedException("DefaultRoadManager does not support Road Transitions, use MultiRoadManager instead!");
        public void RegisterSwitch(Vector2Int entry, Vector2Int point, Vector2Int exit, Road roadEntry, Road roadExit) => throw new System.NotImplementedException("DefaultRoadManager does not support Road Transitions, use MultiRoadManager instead!");

        public void Block(IEnumerable<Vector2Int> points, Road road = null)
        {
            List<Vector2Int> blocked = new List<Vector2Int>();
            foreach (var point in points)
            {
                if (!_blocked.Contains(point))
                    blocked.Add(point);
                _blocked.Add(point);
            }
            _network.Block(blocked);
        }
        public void Unblock(IEnumerable<Vector2Int> points, Road road = null)
        {
            List<Vector2Int> unblocked = new List<Vector2Int>();
            foreach (var point in points)
            {
                _blocked.Remove(point);
                if (!_blocked.Contains(point))
                    unblocked.Add(point);
            }
            _network.Unblock(unblocked);
        }

        public bool CheckRequirement(Vector2Int point, RoadRequirement requirement)
        {
            if (!HasPoint(point))
                return requirement.Check(point, null, null);

            if (_network.TryGetRoad(point, out Road road, out string stage))
            {
                return requirement.Check(point, road, stage);
            }
            else
            {
                return false;
            }
        }

        #region Saving
        public string SaveData()
        {
            return JsonUtility.ToJson(_network.SaveData());
        }

        public void LoadData(string json)
        {
            _network.LoadData(JsonUtility.FromJson<RoadNetwork.RoadsData>(json), _blocked);
        }
        #endregion
    }
}