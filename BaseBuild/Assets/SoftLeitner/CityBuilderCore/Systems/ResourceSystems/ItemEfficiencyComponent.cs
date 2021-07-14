using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// building component that influences efficiency based on what items it has access to<br/>
    /// items are consumed over time and efficiency drops if they run out
    /// </summary>
    public class ItemEfficiencyComponent : BuildingComponent, IEfficiencyFactor, IItemReceiver
    {
        public override string Key => "IEF";

        public ItemRecipient[] Recipients;

        public bool IsWorking => Recipients.All(r => r.HasAccess);
        public float Factor => (float)Recipients.Where(r => r.HasAccess).Count() / Recipients.Length;

        public BuildingComponentReference<IItemReceiver> Reference { get; set; }

        public int Priority => 1000;

        private IGameSettings _settings;

        private void Start()
        {
            _settings = Dependencies.Get<IGameSettings>();
        }

        private void Update()
        {
            Recipients.ForEach(r => r.Update(_settings.ItemsMultiplier));
        }

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            Reference = registerTrait<IItemReceiver>(this);
        }
        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            replaceTrait<IItemReceiver>(this, replacement.GetBuildingComponent<ItemEfficiencyComponent>());
        }
        public override void TerminateComponent()
        {
            base.TerminateComponent();

            deregisterTrait<IItemReceiver>(this);
        }

        public int GetItemCapacityRemaining(Item item)
        {
            var recipient = Recipients.FirstOrDefault(r => r.Item == item);
            if (recipient == null)
                return 0;

            return recipient.Storage.GetItemCapacityRemaining(item);
        }

        public void Receive(ItemStorage storage)
        {
            foreach (var item in storage.GetItem().ToList())
            {
                var recipient = Recipients.FirstOrDefault(r => r.Item == item);
                if (recipient == null)
                    continue;

                storage.MoveItemsTo(recipient.Storage, item);
            }
        }

        public IEnumerable<ItemQuantity> GetItems()
        {
            List<ItemQuantity> items = new List<ItemQuantity>();

            foreach (var recipient in Recipients)
            {
                items.Add(new ItemQuantity(recipient.Item, recipient.Storage.GetItemQuantity(recipient.Item)));
            }

            return items;
        }

        #region Saving
        [Serializable]
        public class ItemsEfficiencyData
        {
            public ItemRecipient.ItemsRecipientData[] Recipients;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new ItemsEfficiencyData()
            {
                Recipients = Recipients.Select(r => r.SaveData()).ToArray()
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<ItemsEfficiencyData>(json);

            for (int i = 0; i < data.Recipients.Length; i++)
            {
                Recipients[i].LoadData(data.Recipients[i]);
            }
        }
        #endregion
    }
}