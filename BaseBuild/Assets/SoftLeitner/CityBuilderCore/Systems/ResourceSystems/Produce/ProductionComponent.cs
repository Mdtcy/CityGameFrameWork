using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// building component that periodically consumes and produces items<br/>
    /// production time is only started once the consumption items are all there<br/>
    /// consumption items have to be provided by others, produced items get shipped with <see cref="DeliveryWalker"/>
    /// </summary>
    public class ProductionComponent : ProgressComponent, IProductionComponent
    {
        public override string Key => "PRD";

        public enum ProductionState
        {
            Idle = 0,//waiting for raw materials in consumers
            Working = 10,//progress going up according to efficiency
            Done = 20//waiting for producers to deposit goods
        }

        public ItemConsumer[] ItemsConsumers;
        public ItemProducer[] ItemsProducers;

        public ManualDeliveryWalkerSpawner DeliveryWalkers;

        public int Priority => 1000;
        public ItemConsumer[] Consumers => ItemsConsumers;
        public ItemProducer[] Producers => ItemsProducers;

        public BuildingComponentReference<IItemReceiver> Reference { get; set; }

        private ProductionState _productionState;
        private bool _isProgressing;

        private void Awake()
        {
            DeliveryWalkers.Initialize(Building);
        }
        private void Start()
        {
            this.StartChecker(checkDelivery);
        }
        private void Update()
        {
            updateProduction();
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            Reference = registerTrait<IItemReceiver>(this);
        }
        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            var productionReplacement = replacement.GetBuildingComponent<IProductionComponent>();

            replaceTrait<IItemReceiver>(this, productionReplacement);

            if (productionReplacement == null)
                return;

            foreach (var itemsProducer in ItemsProducers)
            {
                var producerReplacement = productionReplacement.Producers.FirstOrDefault(p => p.Items.Item == itemsProducer.Items.Item);
                if (producerReplacement != null)
                    itemsProducer.Storage.MoveItemsTo(producerReplacement.Storage);
            }
        }
        public override void TerminateComponent()
        {
            base.TerminateComponent();

            deregisterTrait<IItemReceiver>(this);
        }

        private void updateProduction()
        {
            switch (_productionState)
            {
                case ProductionState.Idle:
                    if (ItemsConsumers.All(c => c.HasItems))
                    {
                        _productionState = ProductionState.Working;
                    }
                    break;
                case ProductionState.Working:
                    bool isProgressing = Building.Efficiency > 0f;
                    if (_isProgressing != isProgressing)
                    {
                        _isProgressing = isProgressing;
                        IsProgressing?.Invoke(_isProgressing);
                    }

                    if (addProgress(Building.Efficiency))
                    {
                        foreach (var consumer in ItemsConsumers)
                        {
                            consumer.Consume();
                        }

                        _productionState = ProductionState.Done;
                        _isProgressing = false;
                        IsProgressing?.Invoke(false);
                    }
                    break;
                case ProductionState.Done:
                    if (ItemsProducers.All(p => p.FitsItems))
                    {
                        foreach (var itemsProducer in ItemsProducers)
                        {
                            itemsProducer.Produce();
                        }
                        _productionState = ProductionState.Idle;
                        resetProgress();
                    }
                    break;
                default:
                    break;
            }
        }

        private void checkDelivery()
        {
            if (!DeliveryWalkers.HasWalker)
                return;

            if (!Building.IsWorking)
                return;

            if (!Building.HasAccessPoint(DeliveryWalkers.Prefab.PathType, DeliveryWalkers.Prefab.PathTag))
                return;

            foreach (var producer in ItemsProducers)
            {
                if (!producer.HasItem)
                    continue;

                DeliveryWalkers.Spawn(walker =>
                {
                    walker.StartDelivery(producer.Storage);
                });
            }
        }

        public int GetItemCapacityRemaining(Item item)
        {
            var consumer = getConsumer(item);
            if (consumer == null)
                return 0;
            return consumer.Storage.GetItemCapacityRemaining(item);
        }

        public void Receive(ItemStorage storage)
        {
            foreach (var item in storage.GetItem().ToList())
            {
                var consumer = getConsumer(item);
                if (consumer == null)
                    continue;

                storage.MoveItemsTo(consumer.Storage);
            }
        }

        private ItemConsumer getConsumer(Item item)
        {
            return ItemsConsumers.FirstOrDefault(c => c.Items.Item == item);
        }

        #region Saving
        [Serializable]
        public class ProductionData
        {
            public int State;
            public float ProductionTime;
            public ItemStorage.ItemStorageData[] Consumers;
            public ItemStorage.ItemStorageData[] Producers;
            public ManualWalkerSpawnerData SpawnerData;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new ProductionData()
            {
                State = (int)_productionState,
                ProductionTime = _progressTime,
                Consumers = ItemsConsumers.Select(c => c.Storage.SaveData()).ToArray(),
                Producers = ItemsProducers.Select(c => c.Storage.SaveData()).ToArray(),
                SpawnerData = DeliveryWalkers.SaveData()
            }); ;
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<ProductionData>(json);

            _productionState = (ProductionState)data.State;
            _progressTime = data.ProductionTime;
            for (int i = 0; i < ItemsConsumers.Length; i++)
            {
                ItemsConsumers[i].Storage.LoadData(data.Consumers[i]);
            }
            for (int i = 0; i < ItemsProducers.Length; i++)
            {
                ItemsProducers[i].Storage.LoadData(data.Producers[i]);
            }
            DeliveryWalkers.LoadData(data.SpawnerData);
        }
        #endregion
    }
}