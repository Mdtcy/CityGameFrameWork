using UnityEngine;

namespace CityBuilderCore
{
    public class TimedReplacementComponent : BuildingComponent
    {
        public override string Key => "TRP";

        public float Duration;
        public Building Prefab;

        private float _passed;

        private void Update()
        {
            _passed += Time.deltaTime * Building.Efficiency;
            if (_passed >= Duration)
                Building.Replace(Prefab);
        }
    }
}
