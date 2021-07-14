using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// bundels of items for whenever instead of a specific item just a general type of item is needed<br/>
    /// eg people need food not just potatoes specifically
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(ItemCategory))]
    public class ItemCategory : ScriptableObject
    {
        public string Key;
        public string NameSingular;
        public string NamePlural;
        public Item[] Items;

        public string GetName(int quantity)
        {
            if (quantity > 1)
                return $"{quantity} {NamePlural}";
            else
                return NameSingular;
        }
    }
}