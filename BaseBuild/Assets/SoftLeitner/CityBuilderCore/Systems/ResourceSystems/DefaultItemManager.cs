using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// default implementation for the resource systems<br/>
    /// <para>
    /// manages global items and all the logistics of moving items between buildings
    /// </para>
    /// </summary>
    public class DefaultItemManager : MonoBehaviour, IGlobalStorage, IGiverPathfinder, IReceiverPathfinder, IItemsDispenserManager
    {
        [Tooltip("global item storage")]
        public ItemStorage ItemStorage;
        [Tooltip("items added to global storage when the game starts")]
        public ItemQuantity[] StartItems;

        public ItemStorage Items => ItemStorage;

        private List<IItemsDispenser> _dispensers = new List<IItemsDispenser>();

        protected virtual void Awake()
        {
            Dependencies.Register<IGlobalStorage>(this);
            Dependencies.Register<IGiverPathfinder>(this);
            Dependencies.Register<IReceiverPathfinder>(this);
            Dependencies.Register<IItemsDispenserManager>(this);
        }

        protected virtual void Start()
        {
            StartItems.ForEach(i => ItemStorage.AddItems(i.Item, i.Quantity));
        }

        public void Add(IItemsDispenser dispenser)
        {
            _dispensers.Add(dispenser);
        }
        public void Remove(IItemsDispenser dispenser)
        {
            _dispensers.Remove(dispenser);
        }

        public IItemsDispenser GetDispenser(string key, Vector3 position, float maxDistance)
        {
            return _dispensers
                .Where(d => d.Key == key)
                .Select(d => Tuple.Create(d, Vector3.Distance(d.Position, position)))
                .Where(d => d.Item2 < maxDistance)
                .OrderBy(d => d.Item2)
                .FirstOrDefault()?.Item1;
        }
        public bool HasDispenser(string key, Vector3 position, float maxDistance)
        {
            return _dispensers
                .Where(d => d.Key == key)
                .Select(d => Tuple.Create(d, Vector3.Distance(d.Position, position)))
                .Where(d => d.Item2 < maxDistance)
                .Any();
        }

        public BuildingComponentPath<IItemGiver> GetGiverPath(IBuilding building, Vector2Int? currentPoint, ItemQuantity items, float maxDistance, PathType pathType, object pathTag)
        {
            if (building == null || items == null)
                return null;

            List<BuildingComponentReference<IItemGiver>> candidates = new List<BuildingComponentReference<IItemGiver>>();
            foreach (var reference in Dependencies.Get<IBuildingManager>().GetBuildingTraitReferences<IItemGiver>())
            {
                if (reference.Instance.Building == building)
                    continue;//dont deliver to self

                var distance = Vector2.Distance(reference.Instance.Building.WorldCenter, building.WorldCenter);
                if (distance > maxDistance)
                    continue;//too far away

                if (!reference.Instance.HasItems(items.Item, items.Quantity))
                    continue;//does not have the items

                candidates.Add(reference);
            }

            if (candidates.Count == 1)
            {
                var path = PathHelper.FindPath(building, currentPoint, candidates[0].Instance.Building, pathType, pathTag);
                if (path == null)
                    return null;

                return new BuildingComponentPath<IItemGiver>(candidates[0], path);
            }
            else if (candidates.Count > 1)
            {
                float maxScore = float.MinValue;
                BuildingComponentReference<IItemGiver> maxScoreGiver = null;
                WalkingPath maxScorePath = null;

                foreach (var giver in candidates)
                {
                    var path = PathHelper.FindPath(building, currentPoint, giver.Instance.Building, pathType, pathTag);
                    if (path == null)
                        continue;

                    var score = maxDistance - path.Length;
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxScoreGiver = giver;
                        maxScorePath = path;
                    }
                }

                if (maxScoreGiver != null)
                    return new BuildingComponentPath<IItemGiver>(maxScoreGiver, maxScorePath);
            }

            return null;
        }

        public BuildingComponentPath<IItemReceiver> GetReceiverPath(IBuilding building, Vector2Int? currentPoint, ItemQuantity items, float maxDistance, PathType pathType, object pathTag, int currentPriority = 0)
        {
            if (building == null || items == null)
                return null;

            List<BuildingComponentReference<IItemReceiver>> candidates = new List<BuildingComponentReference<IItemReceiver>>();
            foreach (var reference in Dependencies.Get<IBuildingManager>().GetBuildingTraitReferences<IItemReceiver>())
            {
                if (reference.Instance.Building == building)
                    continue;//dont deliver to self

                if (reference.Instance.Priority <= currentPriority)
                    continue;//receiver has same or lower priority then the current storage

                var distance = Vector2.Distance(reference.Instance.Building.WorldCenter, building.WorldCenter);
                if (distance > maxDistance)
                    continue;//too far away

                var missing = reference.Instance.GetItemCapacityRemaining(items.Item) / (float)items.Item.UnitSize;
                if (missing < 1f)
                    continue;//not enough space remaining

                candidates.Add(reference);
            }

            if (candidates.Count == 1)
            {
                var path = PathHelper.FindPath(building, currentPoint, candidates[0].Instance.Building, pathType, pathTag);
                if (path == null)
                    return null;

                return new BuildingComponentPath<IItemReceiver>(candidates[0], path);
            }
            else if (candidates.Count > 1)
            {
                float maxScore = float.MinValue;
                BuildingComponentReference<IItemReceiver> maxScoreReceiver = null;
                WalkingPath maxScorePath = null;

                foreach (var receiver in candidates)
                {
                    var path = PathHelper.FindPath(building, currentPoint, receiver.Instance.Building, pathType, pathTag);
                    if (path == null)
                        continue;

                    var unitsMissing = (float)receiver.Instance.GetItemCapacityRemaining(items.Item) / items.Item.UnitSize;
                    var score = unitsMissing * 2f - path.Length;
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxScoreReceiver = receiver;
                        maxScorePath = path;
                    }
                }

                if (maxScoreReceiver != null)
                    return new BuildingComponentPath<IItemReceiver>(maxScoreReceiver, maxScorePath);
            }

            return null;
        }
    }
}