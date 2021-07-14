using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// addon that transfers its scale and rotation to the attached building<br/>
    /// this can be used to attach animations to any building by animating the addon
    /// </summary>
    public class BuildingAddonTransformer : BuildingAddon
    {
        private Vector3 _pivotScale;
        private Quaternion _pivotRotation;

        public override void InitializeAddon()
        {
            base.InitializeAddon();

            _pivotScale = Building.Pivot.localScale;
            _pivotRotation = Building.Pivot.localRotation;
        }

        public override void Update()
        {
            base.Update();

            Building.Pivot.localScale = Vector3.Scale(_pivotScale, transform.localScale);
            Building.Pivot.localRotation = _pivotRotation * transform.localRotation;
        }
    }
}