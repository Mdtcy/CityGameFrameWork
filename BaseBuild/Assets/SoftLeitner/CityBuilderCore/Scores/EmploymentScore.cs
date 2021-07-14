using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// employment percentage for a certain population
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(EmploymentScore))]
    public class EmploymentScore : Score
    {
        public Population Population;

        public override int Calculate() => Mathf.RoundToInt(Dependencies.Get<IEmploymentManager>().GetEmploymentRate(Population) * 100f);
    }
}