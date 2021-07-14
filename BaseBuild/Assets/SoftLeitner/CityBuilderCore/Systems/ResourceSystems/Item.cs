using UnityEngine;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(Item))]
    public class Item : KeyedObject
    {
        [Tooltip("display name")]
        public string Name;
        [Tooltip("priority of the item in logistics")]
        public int Priority;
        [Tooltip("how many items one unit stands for(eg storages capped to 1 unit can store 100 potatoes but only 1 block of granite)")]
        public int UnitSize;
        [Tooltip("icon displayed in ui")]
        public Sprite Icon;
        [Tooltip("material used in visualizers")]
        public Material Material;
        [Tooltip("visuals used in visualizers")]
        public StorageQuantityVisual[] Visuals;
    }
}