using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// averages the layer values accross all building in a category
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(AverageLayerScore))]
    public class AverageLayerScore : Score
    {
        public BuildingCategory BuildingCategory;
        public Layer Layer;

        public override int Calculate()
        {
            return Mathf.RoundToInt(Dependencies.Get<IBuildingManager>().GetBuildings(BuildingCategory)
                .Select(b => (float)Dependencies.Get<ILayerManager>().GetValue(b.Point, Layer))
                .DefaultIfEmpty()
                .Average());
        }
    }
}