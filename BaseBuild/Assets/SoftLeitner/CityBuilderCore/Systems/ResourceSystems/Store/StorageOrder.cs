using System;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class StorageOrder
    {
        public Item Item;
        public StorageOrderMode Mode;
        [Tooltip("relative to total storage capacity from 0 to 1")]
        public float Ratio;
    }
}