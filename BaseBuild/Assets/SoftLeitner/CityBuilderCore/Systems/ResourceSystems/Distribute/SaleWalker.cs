using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// roams around and takes note of what items the <see cref="IItemRecipient"/> it envounters need and supplies them if it carries the item
    /// </summary>
    public class SaleWalker : BuildingComponentWalker<IItemRecipient>
    {
        public ItemStorage Storage;

        public override ItemStorage ItemStorage => Storage;
        public List<Item> Wishlist => _wishlist;

        private List<Item> _wishlist = new List<Item>();

        public void StartSelling(ItemStorage storage)
        {
            _wishlist.Clear();
            storage.MoveItemsTo(Storage);
        }

        public void Empty(ItemStorage other)
        {
            Storage.MoveItemsTo(other);
        }

        protected override void onComponentEntered(IItemRecipient itemRecipient)
        {
            base.onComponentEntered(itemRecipient);

            if (itemRecipient != null)
            {
                foreach (var recipient in itemRecipient.ItemsRecipients)
                {
                    recipient.Fill(Storage);

                    if (!_wishlist.Contains(recipient.Item))
                        _wishlist.Add(recipient.Item);
                }
            }

            if (itemRecipient.ItemsCategoryRecipients != null)
            {
                foreach (var recipient in itemRecipient.ItemsCategoryRecipients)
                {
                    recipient.Fill(Storage);

                    foreach (var item in recipient.ItemCategory.Items)
                    {
                        if (!_wishlist.Contains(item))
                            _wishlist.Add(item);
                    }
                }
            }
        }

        public override string GetDebugText() => Storage.GetDebugText();

        #region Saving
        [Serializable]
        public class SaleWalkerData : RoamingWalkerData
        {
            public ItemStorage.ItemStorageData Storage;
            public string[] Wishlist;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new SaleWalkerData()
            {
                WalkerData = savewalkerData(),
                State = (int)_state,
                Storage = Storage.SaveData(),
                Wishlist = _wishlist.Select(w => w.Key).ToArray()
            });
        }
        public override void LoadData(string json)
        {
            base.LoadData(json);

            var data = JsonUtility.FromJson<SaleWalkerData>(json);
            var items = Dependencies.Get<IKeyedSet<Item>>();

            Storage.LoadData(data.Storage);

            _wishlist = data.Wishlist.Select(k => items.GetObject(k)).ToList();
        }
        #endregion
    }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class ManualSaleWalkerSpawner : ManualWalkerSpawner<SaleWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class CyclicSaleWalkerSpawner : CyclicWalkerSpawner<SaleWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class PooledSaleWalkerSpawner : PooledWalkerSpawner<SaleWalker> { }
}