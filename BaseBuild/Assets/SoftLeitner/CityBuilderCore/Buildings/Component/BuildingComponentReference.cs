using System;
using System.Linq;

namespace CityBuilderCore
{
    public class BuildingComponentReference<T>
    where T : IBuildingComponent
    {
        public T Instance { get; set; }
        public bool HasInstance => Instance as UnityEngine.Object;

        public BuildingComponentReference(T component)
        {
            Instance = component;
        }

        #region Saving
        public BuildingComponentReferenceData GetData() => HasInstance ? new BuildingComponentReferenceData() { BuildingId = Instance.Building.Id.ToString(), ComponentKey = Instance.Key } : null;
    }
    [Serializable]
    public class BuildingComponentReferenceData
    {
        public string BuildingId;
        public string ComponentKey;

        public BuildingComponentReference<T> GetReference<T>() where T : IBuildingTrait<T>
        {
            if (string.IsNullOrWhiteSpace(BuildingId))
                return null;
            Guid id = new Guid(BuildingId);
            return Dependencies.Get<IBuildingManager>().GetBuildingTraitReferences<T>().FirstOrDefault(r => r.Instance.Building.Id == id && r.Instance.Key == ComponentKey);
        }
    }
    #endregion
}