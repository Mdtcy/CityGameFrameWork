using UnityEngine;

namespace CityBuilderCore.Tests
{
    public class DebugItemStorer : MonoBehaviour
    {
        public StorageComponent Target;
        public ItemQuantity Items;

        private void Start()
        {
            this.Delay(5, () =>
             {
                 Target.Storage.AddItems(Items.Item, Items.Quantity);
             });
        }
    }
}