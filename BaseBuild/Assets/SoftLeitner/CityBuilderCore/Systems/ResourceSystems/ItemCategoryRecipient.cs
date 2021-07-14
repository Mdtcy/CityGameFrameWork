﻿using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// receives any item of a specified category and uses it up over time<br/>
    /// </summary>
    [Serializable]
    public class ItemCategoryRecipient
    {
        public ItemCategory ItemCategory;
        public ItemStorage Storage;
        public float ConsumptionInterval;

        public bool HasAccess => ItemCategory.Items.Any(i => Storage.HasItem(i));

        public event Action Changed;
        public event Action Gained;
        public event Action Lost;

        private float _currentTime;

        public void Update(float multiplier)
        {
            if (!HasAccess)
                return;

            _currentTime += Time.deltaTime;

            if (_currentTime < ConsumptionInterval * multiplier)
                return;

            _currentTime = 0f;

            foreach (var item in ItemCategory.Items)
            {
                if (Storage.HasItem(item))
                {
                    Storage.RemoveItems(item, 1);
                    break;
                }
            }

            Changed?.Invoke();
            if (!HasAccess)
                Lost?.Invoke();
        }

        public void Fill(ItemStorage storage)
        {
            var hadAccess = HasAccess;

            foreach (var item in ItemCategory.Items)
            {
                storage.MoveItemsTo(Storage, item);
            }

            Changed?.Invoke();
            if (!hadAccess && HasAccess)
                Gained?.Invoke();
        }

        #region Saving
        [Serializable]
        public class ItemsCategoryRecipientData
        {
            public string Key;
            public float CurrentTime;
            public ItemStorage.ItemStorageData Storage;
        }

        public ItemsCategoryRecipientData SaveData() => new ItemsCategoryRecipientData() { Key = ItemCategory.Key, CurrentTime = _currentTime, Storage = Storage.SaveData() };
        public void LoadData(ItemsCategoryRecipientData data)
        {
            _currentTime = data.CurrentTime;
            Storage.LoadData(data.Storage);
        }
        #endregion
    }
}