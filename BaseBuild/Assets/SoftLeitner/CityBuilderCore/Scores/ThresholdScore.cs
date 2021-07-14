using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// transform a score by defining thresholds
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(ThresholdScore))]
    public class ThresholdScore : Score
    {
        public class ThresholdItem
        {
            public int Threshold;
            public int Value;
        }

        public Score Score;
        public ThresholdItem[] Items;

        public override int Calculate()
        {
            int score = Score.Calculate();
            int value = 0;

            foreach (var item in Items)
            {
                if (score >= item.Threshold)
                    value = item.Value;
            }

            return value;
        }
    }
}