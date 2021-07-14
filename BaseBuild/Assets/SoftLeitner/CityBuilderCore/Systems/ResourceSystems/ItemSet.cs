using UnityEngine;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/Sets/" + nameof(ItemSet))]
    public class ItemSet : KeyedSet<Item> { }
}