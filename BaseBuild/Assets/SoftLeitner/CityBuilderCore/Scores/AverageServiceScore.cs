using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// averages the service values accross all building in a category
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(AverageServiceScore))]
    public class AverageServiceScore : Score
    {
        public BuildingCategory BuildingCategory;
        public Service Service;

        public override int Calculate()
        {
            return Mathf.RoundToInt(Dependencies.Get<IBuildingManager>().GetBuildings(BuildingCategory)
                .SelectMany(b => b.GetBuildingComponents<IServiceRecipient>())
                .Where(s => s.HasServiceValue(Service))
                .Select(s => s.GetServiceValue(Service))
                .DefaultIfEmpty()
                .Average());
        }
    }
}