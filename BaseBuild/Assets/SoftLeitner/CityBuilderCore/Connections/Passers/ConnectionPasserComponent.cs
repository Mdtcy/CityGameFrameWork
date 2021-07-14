using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// influences building efficiency from its connection value
    /// </summary>
    public class ConnectionPasserComponent : BuildingComponent, IConnectionPasser, IEfficiencyFactor
    {
        public override string Key => "CPC";

        public Connection Connection;
        [Tooltip("the minimum efficiency returned so the building does not stall even in a bad position")]
        public float MinValue = 0;
        [Tooltip("the connection value needed to reach max efficiency")]
        public int MaxConnectionValue = 10;

        public event Action<PointsChanged<IConnectionPasser>> PointsChanged;
        public PointValueEvent PointValueChanged;
        public BoolEvent IsWorkingChanged;

        Connection IConnectionPasser.Connection => Connection;

        public bool IsWorking => _value > 0;
        public float Factor => Mathf.Max(MinValue, Mathf.Min(1f, _value / (float)MaxConnectionValue));

        private int _value;

        public override void InitializeComponent()
        {
            base.InitializeComponent();

            Dependencies.Get<IConnectionManager>().Register(this);
        }
        public override void OnReplacing(IBuilding replacement)
        {
            base.OnReplacing(replacement);

            Dependencies.Get<IConnectionManager>().Deregister(this);
            var passerReplacement = replacement.GetBuildingComponents<ConnectionPasserComponent>().FirstOrDefault(c => c.Connection == Connection);
            if (passerReplacement != null)
                Dependencies.Get<IConnectionManager>().Register(passerReplacement);
        }
        public override void TerminateComponent()
        {
            base.TerminateComponent();

            Dependencies.Get<IConnectionManager>().Deregister(this);
        }

        public IEnumerable<Vector2Int> GetPoints() => Building.GetPoints();

        public void ValueChanged(Vector2Int point, int value)
        {
            PointValueChanged?.Invoke(point, value);
            if (point == Building.Point)
            {
                bool wasWorking = IsWorking;

                _value = value;

                if (wasWorking != IsWorking)
                    IsWorkingChanged?.Invoke(IsWorking);
            }
        }
    }
}