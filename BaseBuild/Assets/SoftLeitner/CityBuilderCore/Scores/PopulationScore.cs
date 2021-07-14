using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// quantity of a certain population
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(PopulationScore))]
    public class PopulationScore : Score
    {
        public Population Population;

        public override int Calculate()
        {
            return Dependencies.Get<IPopulationManager>().GetQuantity(Population);
        }
    }
}