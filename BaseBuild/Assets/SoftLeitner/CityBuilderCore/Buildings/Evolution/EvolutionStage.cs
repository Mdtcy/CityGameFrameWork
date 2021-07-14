using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class EvolutionStage
    {
        public BuildingInfo BuildingInfo;

        [Tooltip("evolution requires services(water, education, ...)")]
        public Service[] Services;
        [Tooltip("evolution requires specific items(pottery, ...)")]
        public Item[] Items;
        [Tooltip("evolution requires a number of a type of item(2 types of food for example)")]
        public ItemCategoryRequirement[] ItemCategoryRequirements;
        [Tooltip("evolution requires some layer value(desirability between 5 and 100)")]
        public LayerRequirement[] LayerRequirements;

        public bool Check(Vector2Int position, Service[] services, Item[] items)
        {
            return Services.All(s => services.Contains(s)) &&
                Items.All(s => items.Contains(s)) &&
                ItemCategoryRequirements.All(s => s.IsFulfilled(items)) &&
                LayerRequirements.All(r => r.IsFulfilled(position));
        }
    }
}