using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// score is building count
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(BuildingScore))]
    public class BuildingScore : Score
    {
        public BuildingInfo Building;

        public override int Calculate()
        {
            return Dependencies.Get<IBuildingManager>().Count(Building);
        }
    }
}