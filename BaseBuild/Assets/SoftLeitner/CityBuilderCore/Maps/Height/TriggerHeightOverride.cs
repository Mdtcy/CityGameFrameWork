﻿using UnityEngine;

namespace CityBuilderCore
{
    public class TriggerHeightOverride : MonoBehaviour
    {
        protected float _height;

        private void Start()
        {
            if (Dependencies.Get<IMap>().IsXY)
                _height = transform.position.z;
            else
                _height = transform.position.y;
        }

        private void OnTriggerEnter2D(Collider2D collider) => enter(collider);
        private void OnTriggerEnter(Collider collider) => enter(collider);
        private void enter(Component collider)
        {
            var overrideHeight = collider.GetComponent<IOverrideHeight>();
            if (overrideHeight != null)
                overrideHeight.HeightOverride = _height;
        }

        private void OnTriggerExit2D(Collider2D collider) => exit(collider);
        private void OnTriggerExit(Collider collider) => exit(collider);
        private void exit(Component component)
        {
            var overrideHeight = component.GetComponent<IOverrideHeight>();
            if (overrideHeight != null)
                overrideHeight.HeightOverride = null;
        }
    }
}
