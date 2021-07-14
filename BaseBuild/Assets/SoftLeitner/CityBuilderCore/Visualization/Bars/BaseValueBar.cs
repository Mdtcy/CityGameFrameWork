using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for in game visuals of building values
    /// </summary>
    public abstract class BaseValueBar : MonoBehaviour
    {
        public IBuilding Building => _buildingReference.Instance;

        protected BuildingReference _buildingReference;
        protected IBuildingValue _value;

        public void Initialize(BuildingReference buildingReference, IBuildingValue value)
        {
            _buildingReference = buildingReference;
            _value = value;

            _buildingReference.Replacing += buildingReferenceReplaced;
        }

        protected virtual void OnDestroy()
        {
            _buildingReference.Replacing -= buildingReferenceReplaced;
        }

        private void buildingReferenceReplaced(IBuilding original, IBuilding replacement)
        {
            transform.SetParent(replacement.Root);
        }

        public bool HasValue() => _value.HasValue(_buildingReference.Instance);
        public float GetValue() => _value.GetValue(_buildingReference.Instance);
    }
}