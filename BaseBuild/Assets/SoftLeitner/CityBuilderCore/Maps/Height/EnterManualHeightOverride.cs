using UnityEngine;

namespace CityBuilderCore
{
    public class EnterManualHeightOverride : MonoBehaviour
    {
        public bool Reset;
        public float Height;

        private void OnTriggerEnter2D(Collider2D collider) => enter(collider);
        private void OnTriggerEnter(Collider collider) => enter(collider);
        private void enter(Component collider)
        {
            var overrideHeight = collider.GetComponent<IOverrideHeight>();
            if (overrideHeight != null)
            {
                if (Reset)
                    overrideHeight.HeightOverride = null;
                else
                    overrideHeight.HeightOverride = Height;
            }
        }
    }
}
