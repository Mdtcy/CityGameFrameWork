using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// multiplies another score
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(MultipliedScore))]
    public class MultipliedScore : Score
    {
        public Score Score;
        public float Multiplier;

        public override int Calculate() => Mathf.RoundToInt(Score.Calculate() * Multiplier);
    }
}