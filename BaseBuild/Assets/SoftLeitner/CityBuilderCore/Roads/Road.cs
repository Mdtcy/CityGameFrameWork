using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(Road))]
    public class Road : KeyedObject
    {
        public string Name;
        [Tooltip("road can change appearance and key according to layer values(prettier roads in more desirable areas for example) otherwise just define one stage with no requirements")]
        public RoadStage[] Stages;
        [Tooltip("Items to be subtracted from GlobalStorage for building")]
        public ItemQuantity[] Cost;
        [Tooltip("determines which structures can reside in the same points as this road(when using multi road manager)")]
        public StructureLevelMask Level;
        [Tooltip("whether parts of the road can be destroyed, disable for roads that are part of the map")]
        public bool IsDestructible = true;

        public TileBase GetTile(Vector2Int position)
        {
            for (int i = Stages.Length - 1; i >= 0; i--)
            {
                var stage = Stages[i];

                if (stage.LayerRequirements.All(r => r.IsFulfilled(position)))
                    return stage.Tile;
            }

            return null;
        }
    }
}