using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// visualizes the progress of a <see cref="ProgressComponent"/> by scaling multiple transforms<br/>
    /// scales are all increased in parallel, can be made more visually interesting using <see cref="Variance"/><br/>
    /// (eg the plants at a farm)
    /// </summary>
    public class ProgressMultiScaler : MonoBehaviour
    {
        public ProgressComponent Component;
        public Transform[] Transforms;
        public float Variance = 0f;
        public Vector3 From = Vector3.zero;
        public Vector3 To = Vector3.one;

        private float[] _variances;

        private void Start()
        {
            Component.ProgressReset += setVariances;
            Component.ProgressChanged += updateProgress;

            setVariances();
            updateProgress(Component.Progress);
        }

        private void setVariances()
        {
            _variances = new float[Transforms.Length];
            for (int i = 0; i < Transforms.Length; i++)
            {
                _variances[i] = Random.Range(1 - Variance / 2, 1 + Variance / 2);
            }
            updateProgress(0f);
        }

        private void updateProgress(float progress)
        {
            for (int i = 0; i < Transforms.Length; i++)
            {
                Transforms[i].localScale = Vector3.Lerp(From, To, progress) * _variances[i];
            }
        }
    }
}