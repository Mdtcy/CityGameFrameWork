using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// visualizes a score in unity ui
    /// </summary>
    public class ScoreVisualizer : MonoBehaviour
    {
        public Score Score;
        public int Maximum = 100;
        public TMPro.TMP_Text NameText;
        public TMPro.TMP_Text ScoreText;
        public RectTransform BarTransform;

        private Vector2 _sizeFull;
        private IScoresCalculator _calculator;

        private void Start()
        {
            if (NameText)
                NameText.text = Score.Name;

            if (BarTransform)
                _sizeFull = BarTransform.sizeDelta;

            _calculator = Dependencies.Get<IScoresCalculator>();
            _calculator.Calculated += scoresCalculated;

            scoresCalculated();
        }

        private void scoresCalculated()
        {
            int value = _calculator.GetValue(Score);

            if (BarTransform)
                BarTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _sizeFull, value / (float)Maximum);

            if (ScoreText)
                ScoreText.text = value.ToString();
        }
    }
}