using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class ItemCategoryRequirement
    {
        [Tooltip("how many different items of the category are needed")]
        public int Quantity;
        public ItemCategory ItemCategory;

        public bool IsFulfilled(Item[] items)
        {
            return items.Where(i => ItemCategory.Items.Contains(i)).Count() >= Quantity;
        }
    }
}