using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for building components implementing <see cref="IProgressComponent"/>
    /// </summary>
    public abstract class ProgressComponent : BuildingComponent, IProgressComponent
    {
        #region IProgressComponent

        public event Action        ProgressReset;
        public event Action<float> ProgressChanged;

        public float Progress => _progressTime / ProgressInterval;

        #endregion

        // 进度间的间隔时间
        public float ProgressInterval;

        public BoolEvent IsProgressing;

        // 0-ProgressInterval之间的时间，大于ProgressInterval时Progress前进
        protected float _progressTime;

        // 获取 100%格式
        public override string GetDebugText() => (Progress * 100f).ToString("F0") + "%";

        /// <summary>
        /// 增加时间，参数是倍数，例如两倍速度就可以是2
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        protected bool addProgress(float multiplier)
        {
            _progressTime = Mathf.Min(ProgressInterval, _progressTime + Time.deltaTime * multiplier);
            ProgressChanged?.Invoke(Progress);
            return _progressTime >= ProgressInterval;
        }

        /// <summary>
        /// 重置ProgressTime，并且触发reset和change事件
        /// </summary>
        protected void resetProgress()
        {
            _progressTime = 0f;
            ProgressReset?.Invoke();
            ProgressChanged?.Invoke(Progress);
        }
    }
}