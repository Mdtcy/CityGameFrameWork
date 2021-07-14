using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for building components implementing <see cref="IProgressComponent"/>
    /// </summary>
    public abstract class ProgressComponent : BuildingComponent, IProgressComponent
    {
        public float ProgressInterval;

        public BoolEvent IsProgressing;

        public event Action ProgressReset;
        public event Action<float> ProgressChanged;

        public float Progress => _progressTime / ProgressInterval;

        protected float _progressTime;

        public override string GetDebugText() => (Progress * 100f).ToString("F0") + "%";

        protected bool addProgress(float multiplier)
        {
            _progressTime = Mathf.Min(ProgressInterval, _progressTime + Time.deltaTime * multiplier);
            ProgressChanged?.Invoke(Progress);
            return _progressTime >= ProgressInterval;
        }

        protected void resetProgress()
        {
            _progressTime = 0f;
            ProgressReset?.Invoke();
            ProgressChanged?.Invoke(Progress);
        }
    }
}