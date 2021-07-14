using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    public class MultiRoadManager : MonoBehaviour, IRoadManager, IRoadPathfinder
    {
        [Serializable]
        public class Network
        {
            public Road Road;
            public Tilemap Tilemap;
        }

        private class BlockedProxy : IRoadPathfinderBlocked
        {
            private MultiRoadManager _manager;

            public BlockedProxy(MultiRoadManager manager)
            {
                _manager = manager;
            }

            public WalkingPath FindPath(Vector2Int start, Vector2Int target, object tag = null) => _manager.FindPathBlocked(start, target, tag);
            public bool HasPoint(Vector2Int point, object tag = null) => _manager.HasPointBlocked(point, tag);
        }

        public Network[] Networks;

        public event Action<PointsChanged<IStructure>> PositionsChanged;

        public bool IsDestructible => true;
        public bool IsDecorator => false;
        public bool IsWalkable => true;
        public bool IsAllowedOnRoads => false;

        public Transform Root => transform;

        public StructureReference StructureReference { get; set; }

        public GridPathfinding DefaultPathfinding { get; private set; }
        public GridPathfinding BlockedPathfinding { get; private set; }

        private List<Vector2Int> _blocked;
        private RoadNetwork _combinedNetwork;
        private Dictionary<Road, RoadNetwork> _roadNetworks;

        protected virtual void Awake()
        {
            Dependencies.Register<IRoadManager>(this);
            Dependencies.Register<IRoadPathfinder>(this);
            Dependencies.Register<IRoadPathfinderBlocked>(new BlockedProxy(this));

            DefaultPathfinding = new GridPathfinding();
            BlockedPathfinding = new GridPathfinding();

            _blocked = new List<Vector2Int>();
            _roadNetworks = new Dictionary<Road, RoadNetwork>();

            _combinedNetwork = new RoadNetwork(null, null);
            Networks.ForEach(r => _roadNetworks.Add(r.Road, new RoadNetwork(r.Road, r.Tilemap)));

            _roadNetworks.Values.ForEach(r =>
            {
                _combinedNetwork.DefaultPathfinding.Add(r.DefaultPathfinding.GetPositions());
                _combinedNetwork.BlockedPathfinding.Add(r.DefaultPathfinding.GetPositions());
            });
        }

        protected virtual void Start()
        {
            _roadNetworks.Values.ForEach(n => n.Register());
        }

        public bool HasPoint(Vector2Int point, Road road = null) => getNetwork(road).DefaultPathfinding.HasPoint(point);

        public void Add(IEnumerable<Vector2Int> points, Road road)
        {
            var validPositions = _roadNetworks[road].Add(points, _blocked);
            if (validPositions == null)
                return;

            _combinedNetwork.DefaultPathfinding.Add(validPositions);
            _combinedNetwork.BlockedPathfinding.Add(validPositions);
        }

        public void Register(IEnumerable<Vector2Int> points, Road road)
        {
            _roadNetworks[road].Register(points, _blocked);
            _combinedNetwork.Register(points, _blocked);
        }
        public void Deregister(IEnumerable<Vector2Int> points, Road road)
        {
            _roadNetworks[road].Deregister(points);
            _combinedNetwork.Deregister(points);
        }

        public void RegisterSwitch(Vector2Int point, Road roadEntry, Road roadExit)
        {
            var networkEntry = _roadNetworks[roadEntry];
            var networkExit = _roadNetworks[roadExit];

            networkEntry.RegisterSwitch(point, networkExit);
            networkExit.RegisterSwitch(point, networkEntry);

            _combinedNetwork.DefaultPathfinding.Add(point);
            _combinedNetwork.BlockedPathfinding.Add(point);
        }
        public void RegisterSwitch(Vector2Int entry, Vector2Int point, Vector2Int exit, Road roadEntry, Road roadExit)
        {
            var networkEntry = _roadNetworks[roadEntry];
            var networkExit = _roadNetworks[roadExit];

            networkEntry.RegisterSwitch(entry, point, exit, networkExit);
            networkExit.RegisterSwitch(exit, point, entry, networkEntry);

            _combinedNetwork.DefaultPathfinding.Add(point);
            _combinedNetwork.BlockedPathfinding.Add(point);
        }

        public void Block(IEnumerable<Vector2Int> points, Road road = null)
        {
            List<Vector2Int> blocked = new List<Vector2Int>();
            foreach (var point in points)
            {
                if (!_blocked.Contains(point))
                    blocked.Add(point);
                _blocked.Add(point);
            }

            if (road == null)
                _roadNetworks.Values.ForEach(m => m.Block(blocked));
            else
                _roadNetworks[road].Block(blocked);
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

            if (road == null)
                _roadNetworks.Values.ForEach(m => m.Unblock(unblocked));
            else
                _roadNetworks[road].Unblock(unblocked);
        }

        public void Remove(IEnumerable<Vector2Int> points)
        {
            _roadNetworks.Values.ForEach(r => r.Remove(points));
        }

        public IEnumerable<Vector2Int> GetPoints() => _combinedNetwork.DefaultPathfinding.GetPositions();
        public bool HasPoint(Vector2Int point) => _combinedNetwork.DefaultPathfinding.HasPoint(point);

        public void CheckLayers(IEnumerable<Vector2Int> points) => _roadNetworks.Values.ForEach(r => r.CheckLayers(points));

        public bool CheckRequirement(Vector2Int point, RoadRequirement requirement)
        {
            if (!HasPoint(point))
                return requirement.Check(point, null, null);

            foreach (var network in _roadNetworks.Values)
            {
                if (network.TryGetRoad(point, out Road road, out string stage))
                {
                    return requirement.Check(point, road, stage);
                }
            }

            return false;
        }

        public string GetName() => "Road";

        public bool HasPoint(Vector2Int point, object tag = null) => getNetwork(tag).DefaultPathfinding.HasPoint(point);
        public bool HasPointBlocked(Vector2Int point, object tag = null) => getNetwork(tag).BlockedPathfinding.HasPoint(point);

        public WalkingPath FindPath(Vector2Int start, Vector2Int target, object tag) => getNetwork(tag).DefaultPathfinding.FindPath(start, target);
        public WalkingPath FindPathBlocked(Vector2Int start, Vector2Int target, object tag) => getNetwork(tag).BlockedPathfinding.FindPath(start, target);

        private RoadNetwork getNetwork(object tag)
        {
            if (tag is Road road && _roadNetworks.ContainsKey(road))
                return _roadNetworks[road];
            return _combinedNetwork;
        }

        #region Saving
        [Serializable]
        public class MultiRoadsData
        {
            public RoadNetwork.RoadsData[] Networks;
        }

        public string SaveData()
        {
            return JsonUtility.ToJson(new MultiRoadsData()
            {
                Networks = _roadNetworks.Values.Select(r => r.SaveData()).ToArray()
            });
        }

        public void LoadData(string json)
        {
            var multiRoadsData = JsonUtility.FromJson<MultiRoadsData>(json);
            var oldPoints = GetPoints();

            foreach (var network in multiRoadsData.Networks)
            {
                _roadNetworks.Values.FirstOrDefault(n => n.Road.Key == network.Key)?.LoadData(network, _blocked);

                foreach (var road in network.Roads)
                {
                    _combinedNetwork.DefaultPathfinding.Add(road.Positions);
                    _combinedNetwork.BlockedPathfinding.Add(road.Positions.Except(_blocked));
                }
            }
        }
        #endregion
    }
}