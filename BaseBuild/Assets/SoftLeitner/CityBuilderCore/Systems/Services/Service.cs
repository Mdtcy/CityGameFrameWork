using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// services are special building values that are filled by walkers and decrease over time
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(Service))]
    public class Service : KeyedObject, IBuildingValue
    {
        public string Name;

        public Layer MultiplierLayer;
        public float MultiplierLayerBottom;
        public float MultiplierLayerTop;

        public bool HasValue(IBuilding building) => building?.HasBuildingComponent<IServiceRecipient>() ?? false;
        public float GetValue(IBuilding building) => building?.GetBuildingComponent<IServiceRecipient>()?.GetServiceValue(this) ?? 0f;
        public float GetMultiplier(IBuilding building)
        {
            if (MultiplierLayer == null)
                return 1f;

            float value = Dependencies.Get<ILayerManager>().GetValue(building.Point, MultiplierLayer) - MultiplierLayerBottom;
            return value / (MultiplierLayerTop - MultiplierLayerBottom);
        }
    }
}