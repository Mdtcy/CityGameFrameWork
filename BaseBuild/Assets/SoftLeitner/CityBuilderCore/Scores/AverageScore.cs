using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// averages other scores
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(AverageScore))]
    public class AverageScore : Score
    {
        public Score[] Scores;

        public override int Calculate() => Mathf.RoundToInt((float)Scores.Average(s => s.Calculate()));
    }
}