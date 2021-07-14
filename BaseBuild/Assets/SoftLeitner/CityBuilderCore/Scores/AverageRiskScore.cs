using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// averages the risk values accross all building in a category
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(AverageRiskScore))]
    public class AverageRiskScore : Score
    {
        public BuildingCategory BuildingCategory;
        public Risk Risk;

        public override int Calculate()
        {
            return Mathf.RoundToInt(Dependencies.Get<IBuildingManager>().GetBuildings(BuildingCategory)
                .SelectMany(b => b.GetBuildingComponents<IRiskRecipient>())
                .Where(r => r.HasRiskValue(Risk))
                .Select(r => 100f - r.GetRiskValue(Risk))
                .DefaultIfEmpty()
                .Average());
        }
    }
}