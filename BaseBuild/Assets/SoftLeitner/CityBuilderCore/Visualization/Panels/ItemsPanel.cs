using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// container in unity ui that generates <see cref="ItemPanel"/> for visualizing stored item quantities/>
    /// </summary>
    public class ItemsPanel : MonoBehaviour
    {
        public ItemPanel ItemPrefab;

        private List<ItemPanel> _panels = new List<ItemPanel>();

        public void SetItems(ItemStorage storage) => SetItems(storage.GetItem().ToList(), storage);
        public void SetItems(List<Item> items, ItemStorage storage) => SetItems(items.Select(i => Tuple.Create(i, storage)).ToList());
        public void SetItems(List<Tuple<Item, ItemStorage>> items)
        {
            for (int i = 0; i < Math.Max(_panels.Count, items.Count); i++)
            {
                var panel = _panels.ElementAtOrDefault(i);
                var item = items.ElementAtOrDefault(i);

                if (item == null)
                {
                    _panels.Remove(panel);
                    Destroy(panel.gameObject);
                    continue;
                }

                if (panel == null)
                {
                    panel = Instantiate(ItemPrefab, transform);
                    _panels.Add(panel);
                }

                panel.SetItem(item.Item1, item.Item2);
            }
        }
        public void SetItems(List<ItemQuantity> items)
        {
            for (int i = 0; i < Math.Max(_panels.Count, items.Count); i++)
            {
                var panel = _panels.ElementAtOrDefault(i);
                var item = items.ElementAtOrDefault(i);

                if (item == null)
                {
                    _panels.Remove(panel);
                    Destroy(panel.gameObject);
                    continue;
                }

                if (panel == null)
                {
                    panel = Instantiate(ItemPrefab, transform);
                    _panels.Add(panel);
                }

                panel.SetItem(item);
            }
        }
    }
}