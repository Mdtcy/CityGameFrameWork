using System;
using UnityEngine;
using UnityEngine.Events;

namespace CityBuilderCore
{
    /// <summary>
    /// walker that retrieves items from <see cref="IItemsDispenser"/> filtered by their key<br/>
    /// does not adjust the path when the dispenser moves<br/>
    /// if it arrives at its destination and the dispenser is out of <see cref="RetrieveDistance"/> it will move again to get close enough
    /// </summary>
    public class ItemsRetrieverWalker : Walker
    {
        public enum ItemsRetrieverWalkerState
        {
            Retrieving = 0,
            Retrieved = 5,
            Returning = 10
        }

        [Tooltip("key of the dispensert this walker targets")]
        public string DispenserKey;
        [Tooltip("maximum distance from home to dispenser")]
        public float MaxDistance = 100;
        [Tooltip("maximum distance for the retriever use a dispenser")]
        public float RetrieveDistance = 1;
        [Tooltip("how long the retriever waits after dispensing")]
        public float RetrieveTime;

        public ItemStorage Storage;

        public UnityEvent Retrieving;

        public override ItemStorage ItemStorage => Storage;

        private ItemsRetrieverWalkerState _state;

        public void StartRetrieving(IItemsDispenser dispenser = null)
        {
            _state = ItemsRetrieverWalkerState.Retrieving;
            retrieve(dispenser, true);
        }

        private void retrieve() => retrieve(null);
        private void retrieve(IItemsDispenser dispenser, bool force = false)
        {
            var worldPosition = Dependencies.Get<IGridPositions>().GetWorldPosition(_current);

            if (!(dispenser as UnityEngine.Object))
                dispenser = Dependencies.Get<IItemsDispenserManager>().GetDispenser(DispenserKey, worldPosition, MaxDistance);

            if (dispenser == null)
            {
                onFinished();
            }
            else
            {
                if (!force && Vector3.Distance(dispenser.Position, worldPosition) < RetrieveDistance)
                {
                    Retrieving?.Invoke();

                    var items = dispenser.Dispense();
                    Storage.AddItems(items.Item, items.Quantity);

                    _state = ItemsRetrieverWalkerState.Retrieved;

                    wait(returnHome, RetrieveTime);
                }
                else
                {
                    var path = PathHelper.FindPath(_current, Dependencies.Get<IGridPositions>().GetGridPosition(dispenser.Position), PathType, PathTag);
                    if (path == null)
                        onFinished();
                    else
                        walk(path, retrieve);
                }
            }
        }

        private void returnHome()
        {
            var path = PathHelper.FindPath(_current, _home.Instance, PathType, PathTag);
            if (path == null)
                onFinished();
            else
                walk(path, 0f);
        }

        public override string GetDebugText() => Storage.GetDebugText();

        #region Saving
        [Serializable]
        public class ItemsRetrieverWalkerData
        {
            public WalkerData WalkerData;
            public int State;
            public ItemStorage.ItemStorageData Storage;
            public ItemQuantity.ItemQuantityData Order;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new ItemsRetrieverWalkerData()
            {
                WalkerData = savewalkerData(),
                Storage = Storage.SaveData(),
                State = (int)_state,
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<ItemsRetrieverWalkerData>(json);

            loadWalkerData(data.WalkerData);

            Storage.LoadData(data.Storage);

            _state = (ItemsRetrieverWalkerState)data.State;

            switch (_state)
            {
                case ItemsRetrieverWalkerState.Retrieving:
                    continueWalk(retrieve);
                    break;
                case ItemsRetrieverWalkerState.Retrieved:
                    continueWait(returnHome);
                    break;
                case ItemsRetrieverWalkerState.Returning:
                    continueWalk();
                    break;
                default:
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class ManualItemsRetrieverWalkerSpawner : ManualWalkerSpawner<ItemsRetrieverWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class CyclicItemsRetrieverWalkerSpawner : CyclicWalkerSpawner<ItemsRetrieverWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class PooledItemsRetrieverWalkerSpawner : PooledWalkerSpawner<ItemsRetrieverWalker> { }
}