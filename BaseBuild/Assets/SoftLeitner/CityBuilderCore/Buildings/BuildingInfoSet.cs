using UnityEngine;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/Sets/" + nameof(BuildingInfoSet))]
    public class BuildingInfoSet : KeyedSet<BuildingInfo> { }
}