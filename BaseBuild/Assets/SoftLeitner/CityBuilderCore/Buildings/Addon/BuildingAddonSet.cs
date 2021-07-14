using UnityEngine;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/Sets/" + nameof(BuildingAddonSet))]
    public class BuildingAddonSet : KeyedSet<BuildingAddon> { }
}