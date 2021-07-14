using System;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class LayerRequirement
    {
        public Layer Layer;
        public int MinValue = int.MinValue;
        public int MaxValue = int.MaxValue;

        public int GetValue(Vector2Int position)
        {
            return Dependencies.Get<ILayerManager>().GetValue(position, Layer);
        }
        public bool CheckValue(int value)
        {
            return value >= MinValue && value <= MaxValue;
        }
        public bool IsFulfilled(Vector2Int position)
        {
            var value = Dependencies.Get<ILayerManager>().GetValue(position, Layer);

            return value >= MinValue && value <= MaxValue;
        }
    }
}