using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// holds the value for a risk and increases it over time<br/>
    /// </summary>
    [Serializable]
    public class RiskRecipient
    {
        [Serializable]
        public class RiskRecipientData
        {
            public string Key;
            public float Value;
        }

        public Risk Risk;
        public float IncreasePerSecond;

        public float Value { get; set; }
        public bool HasTriggered => Value >= 100f;

        public event Action<Risk> Triggered;
        public event Action<Risk> Resolved;

        public void Update(float multiplier)
        {
            if (HasTriggered)
                return;

            Value = Mathf.Min(100f, Value + IncreasePerSecond * multiplier * Time.deltaTime);

            if (HasTriggered)
                Triggered?.Invoke(Risk);
        }

        public void Reduce(float amount)
        {
            var hadTriggered = HasTriggered;

            Value = Mathf.Clamp(Value - amount, 0f, 100f);

            if (hadTriggered && !HasTriggered)
                Resolved?.Invoke(Risk);
            if (!hadTriggered && HasTriggered)
                Triggered?.Invoke(Risk);
        }

        public RiskRecipientData GetData() => new RiskRecipientData() { Key = Risk.Key, Value = Value };
    }
}