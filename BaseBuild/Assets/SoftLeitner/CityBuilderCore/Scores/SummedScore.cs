using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// sums other scores together
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(SummedScore))]
    public class SummedScore : Score
    {
        public Score[] Scores;

        public override int Calculate() => Scores.Sum(s => s.Calculate());
    }
}