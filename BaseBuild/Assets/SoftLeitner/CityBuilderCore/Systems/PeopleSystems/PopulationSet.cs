using UnityEngine;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/Sets/" + nameof(PopulationSet))]
    public class PopulationSet : KeyedSet<Population> { }
}