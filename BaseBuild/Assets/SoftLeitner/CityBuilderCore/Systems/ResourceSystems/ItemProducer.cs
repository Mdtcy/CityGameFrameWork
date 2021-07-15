using System;

namespace CityBuilderCore
{
    /// <summary>
    /// 一个简单的容器，可以检查数量是否合适并添加它到Storage中
    /// simple container that can check if an item quantity fits and adds it to a storage<br/>
    /// eg finished goods in production components
    /// </summary>
    [Serializable]
    public class ItemProducer
    {
        public ItemQuantity Items;
        public ItemStorage Storage;

        public bool FitsItems => Storage.FitsItems(Items.Item, Items.Quantity);

        /// <summary>
        /// 库存中是否有这种Item
        /// </summary>
        public bool HasItem => Storage.HasItem(Items.Item);

        public void Produce()
        {
            Storage.AddItems(Items.Item, Items.Quantity);
        }
    }
}