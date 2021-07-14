using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// sums the values for different buildings<br/>
    /// for example monument scores are added together
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(SummedBuildingScore))]
    public class SummedBuildingScore : Score
    {
        public BuildingEvaluation[] Evaluations;

        public override int Calculate()
        {
            return Evaluations.Sum(i => i.GetEvaluation());
        }
    }
}