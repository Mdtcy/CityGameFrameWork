using UnityEngine;
using UnityEngine.UI;

namespace CityBuilderCore
{
    /// <summary>
    /// unity ui panel for visualizing and editing a <see cref="StorageOrder"/><br/>
    /// only works in combination with the container <see cref="StorageOrdersPanel"/>
    /// </summary>
    public class StorageOrderPanel : MonoBehaviour
    {
        public Image Image;
        public TMPro.TMP_Text Name;
        public TMPro.TMP_Dropdown Mode;
        public TMPro.TMP_InputField Ratio;
        public TMPro.TMP_Text Numbers;

        private StorageOrder _order;

        public void SetOrder(StorageOrder order, ItemStorage storage, bool initiate)
        {
            _order = order;

            string capacityText;
            int capacity = storage.GetItemCapacity(order.Item);
            if (capacity > 0 && capacity < int.MaxValue)
                capacityText = $"/{storage.GetItemCapacity(order.Item, order.Ratio)}";
            else
                capacityText = string.Empty;

            Image.sprite = order.Item.Icon;
            Name.text = order.Item.Name;
            Numbers.text = storage.GetItemQuantity(order.Item) + capacityText;

            if (initiate)
            {
                Mode.SetValueWithoutNotify(toIndex(order.Mode));
                Ratio.SetTextWithoutNotify(order.Ratio.ToString("F2"));
            }
        }

        public void ModeChanged(int index)
        {
            _order.Mode = toMode(index);
        }

        public void RatioChanged(string text)
        {
            if (float.TryParse(text, out float ratio))
            {
                _order.Ratio = ratio;
            }
        }

        private StorageOrderMode toMode(int index)
        {
            switch (index)
            {
                case 0:
                    return StorageOrderMode.Neutral;
                case 1:
                    return StorageOrderMode.Get;
                case 2:
                    return StorageOrderMode.Empty;
                default:
                    return StorageOrderMode.Neutral;
            }
        }

        private int toIndex(StorageOrderMode mode)
        {
            switch (mode)
            {
                case StorageOrderMode.Neutral:
                    return 0;
                case StorageOrderMode.Get:
                    return 1;
                case StorageOrderMode.Empty:
                    return 2;
                default:
                    return 0;
            }
        }
    }
}