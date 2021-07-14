using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// building componnent for risks<br/>
    /// updates risks, executes and resolves them and tries to transfer their values when the building gets replaced
    /// </summary>
    public class RiskerComponent : BuildingComponent, IRiskRecipient
    {
        public override string Key => "RSK";

        public RiskRecipient[] RiskRecipients;

        RiskRecipient[] IRiskRecipient.RiskRecipients => RiskRecipients;

        private IGameSettings _settings;

        private void Start()
        {
            foreach (var riskRecipient in RiskRecipients)
            {
                riskRecipient.Triggered += riskRecipientTriggered;
                riskRecipient.Resolved += riskRecipientResolved;
            }

            _settings = Dependencies.Get<IGameSettings>();
        }

        private void Update()
        {
            foreach (var riskRecipient in RiskRecipients)
            {
                riskRecipient.Update(_settings.RiskMultiplier * riskRecipient.Risk.GetMultiplier(Building));
            }
        }

        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            var riskReplacement = replacement.GetBuildingComponent<IRiskRecipient>();
            if (riskReplacement == null)
                return;

            foreach (var riskRecipient in RiskRecipients)
            {
                var newRecipient = riskReplacement.RiskRecipients.Where(r => r.Risk == riskRecipient.Risk).SingleOrDefault();
                if (newRecipient == null)
                    continue;

                newRecipient.Value = riskRecipient.Value;
            }
        }

        public void ReduceRisk(Risk risk, float amount) => RiskRecipients.Where(r => r.Risk == risk).SingleOrDefault()?.Reduce(amount);
        public bool HasRiskValue(Risk risk) => RiskRecipients.Any(r => r.Risk == risk);
        public float GetRiskValue(Risk risk) => RiskRecipients.FirstOrDefault(r => r.Risk == risk)?.Value ?? 0f;

        private void riskRecipientTriggered(Risk risk) => risk.Execute(this);
        private void riskRecipientResolved(Risk risk) => risk.Resolve(this);

        #region Saving
        [Serializable]
        public class RiskerData
        {
            public RiskRecipient.RiskRecipientData[] RiskRecipients;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new RiskerData()
            {
                RiskRecipients = RiskRecipients.Select(r => r.GetData()).ToArray()
            });
        }
        public override void LoadData(string json)
        {
            base.LoadData(json);

            var data = JsonUtility.FromJson<RiskerData>(json);
            foreach (var recipientData in data.RiskRecipients)
            {
                var recipient = RiskRecipients.FirstOrDefault(r => r.Risk.Key == recipientData.Key);
                if (recipient == null)
                    continue;
                recipient.Value = recipientData.Value;
            }
        }
        #endregion
    }
}