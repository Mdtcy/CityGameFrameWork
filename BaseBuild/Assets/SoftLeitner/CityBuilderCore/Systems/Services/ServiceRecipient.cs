using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// holds the value for a service and decreases it over time<br/>
    /// </summary>
    [Serializable]
    public class ServiceRecipient
    {
        public Service Service;
        public float LossPerSecond;

        public float Value { get; set; }
        public bool HasAccess => Value > 0f;

        public event Action Gained;
        public event Action Lost;

        public void Update(float multiplier)
        {
            if (Value == 0f)
                return;

            Value = Mathf.Max(0f, Value - LossPerSecond * multiplier * Time.deltaTime);

            if (Value == 0f)
                Lost?.Invoke();
        }

        public void Fill(float amount)
        {
            var hadAccess = HasAccess;

            Value = Mathf.Clamp(Value + amount, 0f, 100f);

            if (!hadAccess && HasAccess)
                Gained?.Invoke();
            if (hadAccess && !HasAccess)
                Lost?.Invoke();
        }

        #region Saving
        [Serializable]
        public class ServiceRecipientData
        {
            public string Key;
            public float Value;
        }

        public ServiceRecipientData SaveData() => new ServiceRecipientData() { Key = Service.Key, Value = Value };
        public void LoadData(ServiceRecipientData data)
        {
            Value = data.Value;
        }
        #endregion
    }
}