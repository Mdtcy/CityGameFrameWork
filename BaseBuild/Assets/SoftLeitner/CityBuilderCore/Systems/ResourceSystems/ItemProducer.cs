using System;

namespace CityBuilderCore
{
    /// <summary>
    /// simple container that can check if an item quantity fits and adds it to a storage<br/>
    /// eg finished goods in production components
    /// </summary>
    [Serializable]
    public class ItemProducer
    {
        public ItemQuantity Items;
        public ItemStorage Storage;

        public bool FitsItems => Storage.FitsItems(Items.Item, Items.Quantity);
        public bool HasItem => Storage.HasItem(Items.Item);

        public void Produce()
        {
            Storage.AddItems(Items.Item, Items.Quantity);
        }
    }
}