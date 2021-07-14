using System;
using System.Collections;
using UnityEngine;

namespace CityBuilderCore
{
    public class WaitingState
    {
        public float SetTime;
        public float WaitTime;

        public bool IsFinished => WaitTime >= SetTime;

        public IEnumerator Wait()
        {
            while (!IsFinished)
            {
                yield return null;
                WaitTime += Time.deltaTime;
            }
        }

        #region Saving
        [Serializable]
        public class WaitingData
        {
            public float SetTime;
            public float WaitTime;
        }

        public WaitingData GetData() => new WaitingData()
        {
            SetTime = SetTime,
            WaitTime = WaitTime
        };
        public static WaitingState FromData(WaitingData data)
        {
            if (data == null)
                return null;
            return new WaitingState()
            {
                SetTime = data.SetTime,
                WaitTime = data.WaitTime
            };
        }
        #endregion
    }
}