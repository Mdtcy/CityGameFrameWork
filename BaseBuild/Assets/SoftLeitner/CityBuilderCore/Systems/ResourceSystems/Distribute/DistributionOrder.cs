using System;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class DistributionOrder
    {
        public Item Item;
        [Tooltip("how much of the item to put on stock in relation to the storage capacity(0-1)")]
        public float Ratio;
    }
}