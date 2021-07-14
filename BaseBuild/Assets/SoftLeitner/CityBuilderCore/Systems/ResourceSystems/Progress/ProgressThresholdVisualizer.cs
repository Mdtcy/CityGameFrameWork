using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    public class ProgressThresholdVisualizer : MonoBehaviour
    {
        public ProgressComponent Component;
        [Tooltip("true > show only highest cleared threshold | false > show all cleared thresholds")]
        public bool Swap;
        public ProgressThreshold[] ProgressThresholds;

        private void Start()
        {
            Component.ProgressChanged += updateProgress;

            updateProgress(Component.Progress);
        }

        private void updateProgress(float progress)
        {
            if (Swap)
            {
                ProgressThresholds.ForEach(t => t.GameObject.SetActive(false));
                var activeThreshold = ProgressThresholds.Where(p => progress > p.Value).LastOrDefault();
                if (activeThreshold != null)
                    activeThreshold.GameObject.SetActive(true);
            }
            else
            {
                foreach (var threshold in ProgressThresholds)
                {
                    threshold.GameObject.SetActive(progress > threshold.Value);
                }
            }
        }
    }
}
