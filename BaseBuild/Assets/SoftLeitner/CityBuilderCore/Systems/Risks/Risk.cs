namespace CityBuilderCore
{
    /// <summary>
    /// base class for risks<br/>
    /// risks are special building values that execute when their value hits 0
    /// </summary>
    public abstract class Risk : KeyedObject, IBuildingValue
    {
        public string Name;

        public Layer MultiplierLayer;
        public float MultiplierLayerBottom;
        public float MultiplierLayerTop;

        public abstract void Execute(IRiskRecipient risker);
        public abstract void Resolve(IRiskRecipient risker);

        public bool HasValue(IBuilding building) => building?.HasBuildingComponent<IRiskRecipient>() ?? false;
        public float GetValue(IBuilding building) => building?.GetBuildingComponent<IRiskRecipient>()?.GetRiskValue(this) ?? 0f;
        public float GetMultiplier(IBuilding building)
        {
            if (MultiplierLayer == null)
                return 1f;

            float value = Dependencies.Get<ILayerManager>().GetValue(building.Point, MultiplierLayer) - MultiplierLayerBottom;
            return value / ((MultiplierLayerTop - MultiplierLayerBottom) / 2f);
        }
    }
}