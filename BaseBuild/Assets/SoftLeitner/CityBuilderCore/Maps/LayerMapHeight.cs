using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public class LayerMapHeight : MonoBehaviour, IGridHeights
    {
        [Serializable]
        public class LayerHeight
        {
            [Layer]
            public int Layer;
            public int Height;
        }

        [Layer]
        public int DefaultLayer;
        public LayerHeight[] Heights;

        private Dictionary<int, int> _layers;

        private void Awake()
        {
            Dependencies.Register<IGridHeights>(this);
        }

        private void Start()
        {
            _layers = new Dictionary<int, int>();
            foreach (var height in Heights)
            {
                _layers.Add(height.Height, height.Layer);
            }
        }

        public void SetHeight(Transform transform, Vector3 position, PathType pathType = PathType.Map, float? overrideValue = null)
        {
            var layer = getLayer(overrideValue);

            if (transform.gameObject.layer == layer)
                return;

            transform.gameObject.layer = layer;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = layer;
            }
        }

        private int getLayer(float? height)
        {
            if (!height.HasValue)
                return DefaultLayer;

            if (!_layers.TryGetValue(Mathf.RoundToInt(height.Value), out int layer))
                return DefaultLayer;

            return layer;
        }
    }
}
