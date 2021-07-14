using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// building component that spawns walkers that collect items from <see cref="IGenerationComponent"/><br/>
    /// the collected items are either stored in global storage(set <see cref="Storage"/> to Global) or distributed using a <see cref="DeliveryWalker"/>
    /// </summary>
    public class CollectionComponent : BuildingComponent, IItemGiver
    {
        public override string Key => "COL";

        public ItemStorage Storage;

        public CyclicCollectionWalkerSpawner CollectionWalkers;
        public ManualDeliveryWalkerSpawner DeliveryWalkers;

        public BuildingComponentReference<IItemGiver> Reference { get; set; }

        private void Awake()
        {
            CollectionWalkers.Initialize(Building, onFinished: walkerReturning);
            DeliveryWalkers.Initialize(Building);
        }
        private void Start()
        {
            if (Storage.Mode != ItemStorageMode.Global && DeliveryWalkers.Prefab)
                this.StartChecker(checkDelivery);
        }
        private void Update()
        {
            if (Building.IsWorking)
                CollectionWalkers.Update();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            Reference = registerTrait<IItemGiver>(this);
        }
        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            replaceTrait<IItemGiver>(this, replacement.GetBuildingComponent<CollectionComponent>());
        }
        public override void TerminateComponent()
        {
            base.TerminateComponent();

            deregisterTrait<IItemGiver>(this);
        }

        public override string GetDebugText() => Storage.GetDebugText();

        private void walkerReturning(CollectionWalker walker)
        {
            walker.Storage.MoveItemsTo(Storage);
        }

        private void checkDelivery()
        {
            if (!DeliveryWalkers.HasWalker)
                return;

            if (!Building.IsWorking)
                return;

            if (!Storage.HasItems())
                return;

            if (!Building.HasAccessPoint(DeliveryWalkers.Prefab.PathType, DeliveryWalkers.Prefab.PathTag))
                return;

            DeliveryWalkers.Spawn(walker =>
            {
                walker.StartDelivery(Storage);
            });
        }

        public void Give(ItemStorage storage, Item item, int amount) => Storage.MoveItemsTo(storage, item, amount);
        public bool HasItems(Item item, int amount) => Storage.HasItems(item, amount);
        public void Reserve(Item item, int amount) => Storage.ReserveQuantity(item, amount);
        public void Unreserve(Item item, int amount) => Storage.UnreserveQuantity(item, amount);

        #region Saving
        [Serializable]
        public class CollectionData
        {
            public ItemStorage.ItemStorageData Storage;
            public CyclicWalkerSpawnerData CollectionSpawnerData;
            public ManualWalkerSpawnerData DeliverySpawnerData;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new CollectionData()
            {
                Storage = Storage.SaveData(),
                CollectionSpawnerData = CollectionWalkers.SaveData(),
                DeliverySpawnerData = DeliveryWalkers.SaveData(),
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<CollectionData>(json);

            Storage.LoadData(data.Storage);
            CollectionWalkers.LoadData(data.CollectionSpawnerData);
            DeliveryWalkers.LoadData(data.DeliverySpawnerData);
        }
        #endregion
    }
}