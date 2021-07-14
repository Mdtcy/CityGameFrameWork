using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// building component that stores items and has storage walkers to manage items as follows:<br/>
    /// I:      fill up on items that have been configured as <see cref="StorageOrderMode.Get"/><br/>
    /// II:     deliver to <see cref="IItemReceiver"/> that need items<br/>
    /// III:    get rid of items that have been configured as <see cref="StorageOrderMode.Empty"/><br/>
    /// </summary>
    public class StorageComponent : BuildingComponent, IStorageComponent
    {
        public override string Key => "STG";

        public ItemStorage Storage;
        public StorageOrder[] Orders;

        public ManualStorageWalkerSpawner StorageWalkers;

        public int Priority => 100;

        ItemStorage IStorageComponent.Storage => Storage;
        StorageOrder[] IStorageComponent.Orders => Orders;

        public BuildingComponentReference<IItemReceiver> ReceiverReference { get; set; }
        public BuildingComponentReference<IItemGiver> GiverReference { get; set; }

        BuildingComponentReference<IItemGiver> IBuildingTrait<IItemGiver>.Reference { get => GiverReference; set => GiverReference = value; }
        BuildingComponentReference<IItemReceiver> IBuildingTrait<IItemReceiver>.Reference { get => ReceiverReference; set => ReceiverReference = value; }

        private void Awake()
        {
            StorageWalkers.Initialize(Building, onFinished: storageWalkerReturned);
        }
        private void Start()
        {
            this.StartChecker(checkWorkers);
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            ReceiverReference = registerTrait<IItemReceiver>(this);
            GiverReference = registerTrait<IItemGiver>(this);
        }
        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            var storageReplacement = replacement.GetBuildingComponent<IStorageComponent>();

            replaceTrait<IItemReceiver>(this, storageReplacement);
            replaceTrait<IItemGiver>(this, storageReplacement);
        }
        public override void TerminateComponent()
        {
            base.TerminateComponent();

            deregisterTrait<IItemReceiver>(this);
            deregisterTrait<IItemGiver>(this);
        }

        public override string GetDebugText() => Storage.GetDebugText();

        private void checkWorkers()
        {
            if (!StorageWalkers.HasWalker)
                return;

            if (!Building.IsWorking)
                return;

            if (!Building.HasAccessPoint(StorageWalkers.Prefab.PathType, StorageWalkers.Prefab.PathTag))
                return;

            //GET
            foreach (var order in Orders.Where(o => o.Mode == StorageOrderMode.Get).OrderBy(o => o.Item.Priority))
            {
                var capacity = GetItemCapacityRemaining(order.Item);
                if (capacity <= 0)
                    return;

                var giverPath = Dependencies.Get<IGiverPathfinder>().GetGiverPath(Building, null, new ItemQuantity(order.Item, order.Item.UnitSize), StorageWalkers.Prefab.MaxDistance, StorageWalkers.Prefab.PathType, StorageWalkers.Prefab.PathTag);
                if (giverPath == null)
                    continue;

                StorageWalkers.Spawn(walker => walker.StartGet(giverPath, new ItemQuantity(order.Item, capacity)));
            }
            //SUPPLY
            foreach (var items in Storage.GetItems().OrderBy(i => i.Item.Priority).ToList())
            {
                var receiverPath = Dependencies.Get<IReceiverPathfinder>().GetReceiverPath(Building, null, items, StorageWalkers.Prefab.MaxDistance, StorageWalkers.Prefab.PathType, StorageWalkers.Prefab.PathTag, Priority);
                if (receiverPath == null)
                    continue;

                StorageWalkers.Spawn(walker => walker.StartSupply(receiverPath, Storage, items.Item));
                return;
            }
            //EMPTY
            foreach (var order in Orders.Where(o => o.Mode == StorageOrderMode.Empty).OrderBy(o => o.Item.Priority))
            {
                int capacity = Storage.GetItemCapacityOverflowing(order.Item, order.Ratio);
                if (capacity <= 0)
                    return;

                var receiverPath = Dependencies.Get<IReceiverPathfinder>().GetReceiverPath(Building, null, new ItemQuantity(order.Item, capacity), StorageWalkers.Prefab.MaxDistance, StorageWalkers.Prefab.PathType, StorageWalkers.Prefab.PathTag);
                if (receiverPath == null)
                    continue;

                StorageWalkers.Spawn(walker => walker.StartEmpty(receiverPath, Storage, order.Item, capacity));
                return;
            }
        }

        public int GetItemCapacityRemaining(Item item)
        {
            if (!Building.IsWorking)
                return 0;

            var order = Orders.FirstOrDefault(o => o.Item == item);
            if (order == null || order.Ratio == 0)
                return 0;

            return Storage.GetItemCapacityRemaining(item, order.Ratio);
        }

        public void Receive(ItemStorage storage)
        {
            if (!Building.IsWorking)
                return;

            foreach (var items in storage.GetItems().ToList())
            {
                var order = Orders.FirstOrDefault(o => o.Item == items.Item);
                if (order == null || order.Ratio == 0)
                    continue;

                int quantity = Math.Min(items.Quantity, Storage.GetItemCapacityRemaining(items.Item, order.Ratio));
                int removed = quantity - Storage.AddItems(items.Item, quantity);
                storage.RemoveItems(items.Item, removed);
            }
        }

        public bool HasItems(Item item, int amount)
        {
            if (!Building.IsWorking)
                return false;

            return Storage.HasItems(item, amount);
        }

        public void Reserve(Item item, int amount)
        {
            Storage.ReserveQuantity(item, amount);
        }

        public void Unreserve(Item item, int amount)
        {
            Storage.UnreserveQuantity(item, amount);
        }

        public void Give(ItemStorage storage, Item item, int amount)
        {
            if (!Building.IsWorking)
                return;

            Storage.MoveItemsTo(storage, item, amount);
        }

        private void storageWalkerReturned(StorageWalker walker)
        {
            Receive(walker.Storage);
        }

        #region Saving
        [Serializable]
        public class StorageData
        {
            public ItemStorage.ItemStorageData Storage;
            public ManualWalkerSpawnerData SpawnerData;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new StorageData()
            {
                Storage = Storage.SaveData(),
                SpawnerData = StorageWalkers.SaveData()
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<StorageData>(json);

            Storage.LoadData(data.Storage);
            StorageWalkers.LoadData(data.SpawnerData);
        }
        #endregion
    }
}