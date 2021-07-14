using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// default building for handling evolution
    /// <para>
    /// determines the current evolution stage based on layers, services and items<br/>
    /// if the stage changes it waits for the delay specified in <see cref="IBuildingManager"/> before replacing the building<br/>
    /// evolution components on lower buildings also need the recipients for higher ones in order to evolve<br/>
    /// </para>
    /// </summary>
    public class EvolutionComponent : BuildingComponent, IEvolution
    {
        public override string Key => "EVO";

        public EvolutionSequence EvolutionSequence;

        public ServiceRecipient[] ServiceRecipients;
        public ItemRecipient[] ItemsRecipients;
        public ItemCategoryRecipient[] ItemsCategoryRecipients;

        ServiceRecipient[] IServiceRecipient.ServiceRecipients => ServiceRecipients;
        ItemRecipient[] IItemRecipient.ItemsRecipients => ItemsRecipients;
        ItemCategoryRecipient[] IItemRecipient.ItemsCategoryRecipients => ItemsCategoryRecipients;

        private IGameSettings _settings;
        private EvolutionStage _queuedStage;
        private string _queuedAddon;
        private bool _queueDirection;
        private float _queueTime;

        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            var evolutionReplacement = replacement.GetBuildingComponent<IEvolution>();
            if (evolutionReplacement == null)
                return;

            foreach (var recipient in ServiceRecipients)
            {
                var newRecipient = evolutionReplacement.ServiceRecipients.Where(r => r.Service == recipient.Service).SingleOrDefault();
                if (newRecipient == null)
                    continue;

                newRecipient.Value = recipient.Value;
            }

            foreach (var recipient in ItemsRecipients)
            {
                var newRecipient = evolutionReplacement.ItemsRecipients.Where(r => r.Item == recipient.Item).SingleOrDefault();
                if (newRecipient == null)
                    continue;

                recipient.Storage.MoveItemsTo(newRecipient.Storage);
            }

            foreach (var recipient in ItemsCategoryRecipients)
            {
                var newRecipient = evolutionReplacement.ItemsCategoryRecipients.Where(r => r.ItemCategory == recipient.ItemCategory).SingleOrDefault();
                if (newRecipient == null)
                    continue;

                recipient.Storage.MoveItemsTo(newRecipient.Storage);
            }
        }

        private void Start()
        {
            foreach (var recipient in ServiceRecipients)
            {
                recipient.Gained += CheckEvolution;
                recipient.Lost += CheckEvolution;
            }

            foreach (var recipient in ItemsRecipients)
            {
                recipient.Gained += CheckEvolution;
                recipient.Lost += CheckEvolution;
            }

            foreach (var recipient in ItemsCategoryRecipients)
            {
                recipient.Changed += CheckEvolution;
            }

            _settings = Dependencies.Get<IGameSettings>();

            this.Delay(5, CheckEvolution);
        }
        private void Update()
        {
            foreach (var recipient in ServiceRecipients)
            {
                recipient.Update(_settings.ServiceMultiplier * recipient.Service.GetMultiplier(Building));
            }

            foreach (var recipient in ItemsRecipients)
            {
                recipient.Update(_settings.ItemsMultiplier);
            }

            foreach (var recipient in ItemsCategoryRecipients)
            {
                recipient.Update(_settings.ItemsMultiplier);
            }

            if (_queuedStage?.BuildingInfo != null)
            {
                _queueTime += Time.deltaTime;
                if (_queueTime >= Dependencies.Get<IBuildingManager>().GetEvolutionDelay(_queueDirection))
                    Building.Replace(_queuedStage.BuildingInfo.Prefab);
            }
        }

        public void CheckLayers(IEnumerable<Vector2Int> positions) => CheckEvolution();
        public void CheckEvolution()
        {
            if (!gameObject.scene.isLoaded)
                return;

            if (EvolutionSequence == null)
                return;

            var stage = EvolutionSequence.GetStage(Building.Point,
                ServiceRecipients.Where(r => r.HasAccess).Select(r => r.Service).ToArray(),
                GetItem().ToArray());

            if (_queuedStage != null)
            {
                if (stage == _queuedStage)
                    return;

                _queueTime = 0f;
                _queuedStage = null;
                if (!string.IsNullOrWhiteSpace(_queuedAddon))
                    Building.RemoveAddon(_queuedAddon);
            }

            if (stage != null && stage.BuildingInfo == Building.Info)
                return;

            var direction = EvolutionSequence.GetDirection(Building.Info, stage.BuildingInfo);
            var evolutionManager = Dependencies.Get<IBuildingManager>();

            if (evolutionManager.HasEvolutionDelay(direction))
            {
                _queueDirection = direction;
                _queueTime = 0f;
                _queuedStage = stage;
                _queuedAddon = evolutionManager.AddEvolutionAddon(Building, direction);
            }
            else
            {
                Building.Replace(stage.BuildingInfo.Prefab);
            }
        }

        public void ProvideService(Service service, float amount) => ServiceRecipients.Where(r => r.Service == service).SingleOrDefault()?.Fill(amount);
        public bool HasServiceValue(Service service) => ServiceRecipients.Any(s => s.Service == service);
        public float GetServiceValue(Service service) => ServiceRecipients.FirstOrDefault(r => r.Service == service)?.Value ?? 0f;

        public IEnumerable<Item> GetItem()
        {
            List<Item> items = new List<Item>();

            foreach (var recipient in ItemsRecipients)
            {
                if (!recipient.HasAccess)
                    continue;

                if (!items.Contains(recipient.Item))
                    items.Add(recipient.Item);
            }

            foreach (var recipient in ItemsCategoryRecipients)
            {
                foreach (var item in recipient.Storage.GetItem())
                {
                    if (!items.Contains(item))
                        items.Add(item);
                }
            }

            return items;
        }
        public IEnumerable<ItemQuantity> GetItems()
        {
            List<ItemQuantity> items = new List<ItemQuantity>();

            foreach (var recipient in ItemsRecipients)
            {
                foreach (var recipientItems in recipient.Storage.GetItems())
                {
                    var existingItems = items.FirstOrDefault(i => i.Item == recipientItems.Item);
                    if (existingItems == null)
                        items.Add(recipientItems);
                    else
                        existingItems.Quantity += recipientItems.Quantity;
                }
            }

            foreach (var recipient in ItemsCategoryRecipients)
            {
                foreach (var recipientItems in recipient.Storage.GetItems())
                {
                    var existingItems = items.FirstOrDefault(i => i.Item == recipientItems.Item);
                    if (existingItems == null)
                        items.Add(recipientItems);
                    else
                        existingItems.Quantity += recipientItems.Quantity;
                }
            }
            
            return items;
        }

        public override string GetDescription()
        {
            return EvolutionSequence.GetDescription(Building.Info, _queuedStage?.BuildingInfo, Building.Point, ServiceRecipients.Where(r => r.HasAccess).Select(r => r.Service), GetItem());
        }

        #region Saving
        [Serializable]
        public class EvolutionData
        {
            public ServiceRecipient.ServiceRecipientData[] ServiceRecipients;
            public ItemRecipient.ItemsRecipientData[] ItemsRecipients;
            public ItemCategoryRecipient.ItemsCategoryRecipientData[] ItemsCategoryRecipients;
            public int QueuedStage;
            public string QueuedAddon;
            public bool QueueDirection;
            public float QueueTime;
        }

        public override string SaveData()
        {
            var data = new EvolutionData()
            {
                ServiceRecipients = ServiceRecipients.Select(r => r.SaveData()).ToArray(),
                ItemsRecipients = ItemsRecipients.Select(i => i.SaveData()).ToArray(),
                ItemsCategoryRecipients = ItemsCategoryRecipients.Select(i => i.SaveData()).ToArray()
            };

            if (_queuedStage != null)
            {
                data.QueuedStage = Array.IndexOf(EvolutionSequence.Stages, _queuedStage);
                data.QueuedAddon = _queuedAddon;
                data.QueueDirection = _queueDirection;
                data.QueueTime = _queueTime;
            }
            else
            {
                data.QueuedStage = -1;
            }

            return JsonUtility.ToJson(data);
        }
        public override void LoadData(string json)
        {
            base.LoadData(json);

            var data = JsonUtility.FromJson<EvolutionData>(json);

            foreach (var recipientData in data.ServiceRecipients)
            {
                var recipient = ServiceRecipients.FirstOrDefault(r => r.Service.Key == recipientData.Key);
                if (recipient == null)
                    continue;
                recipient.LoadData(recipientData);
            }

            foreach (var recipientData in data.ItemsRecipients)
            {
                var recipient = ItemsRecipients.FirstOrDefault(r => r.Item.Key == recipientData.Key);
                if (recipient == null)
                    continue;
                recipient.LoadData(recipientData);
            }

            foreach (var recipientData in data.ItemsCategoryRecipients)
            {
                var recipient = ItemsCategoryRecipients.FirstOrDefault(r => r.ItemCategory.Key == recipientData.Key);
                if (recipient == null)
                    continue;
                recipient.LoadData(recipientData);
            }

            if (data.QueuedStage >= 0)
            {
                _queuedStage = EvolutionSequence.Stages[data.QueuedStage];
                _queuedAddon = data.QueuedAddon;
                _queueDirection = data.QueueDirection;
                _queueTime = data.QueueTime;
            }
        }
        #endregion
    }
}