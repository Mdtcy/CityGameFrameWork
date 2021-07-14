using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// 将Item与数量绑定
    /// convenience container that combines an item with a quantity
    /// </summary>
    [Serializable]
    public class ItemQuantity
    {
        public Item Item;
        public int Quantity;
        public float UnitQuantity => (float)Quantity / Item.UnitSize;

        public ItemQuantity()
        {

        }

        public ItemQuantity(Item item, int amount)
        {
            Item = item;
            Quantity = amount;
        }

        public int Remove(int max)
        {
            var count = Mathf.Min(Quantity, max);

            Quantity -= count;

            return count;
        }

        #region Saving
        [Serializable]
        public class ItemQuantityData
        {
            public string Key;
            public int Quantity;
        }

        public ItemQuantityData GetData() => new ItemQuantityData() { Key = Item ? Item.Key : null, Quantity = Quantity };
        public static ItemQuantity FromData(ItemQuantityData data)
        {
            if (data == null)
                return null;
            return new ItemQuantity(data.Key == null ? null : Dependencies.Get<IKeyedSet<Item>>().GetObject(data.Key), data.Quantity);
        }
        #endregion
    }
}