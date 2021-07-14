using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// derives its building efficiency from the layer value at building origin(for example to influence farming speed from fertility layer)
    /// </summary>
    public class LayerEfficiencyComponent : BuildingComponent, IEfficiencyFactor
    {
        public override string Key => "LEF";

        public Layer Layer;
        [Tooltip("the minimum efficiency returned so the building does not stall even in a bad position")]
        public float MinValue = 0;
        [Tooltip("the layer value needed to reach max efficiency")]
        public int MaxLayerValue = 10;

        public bool IsWorking => true;
        public float Factor => Mathf.Max(MinValue, Mathf.Min(1f, Dependencies.Get<ILayerManager>().GetValue(Building.Point, Layer) / (float)MaxLayerValue));
    }
}