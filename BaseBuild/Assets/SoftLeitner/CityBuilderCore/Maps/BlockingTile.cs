using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    [Serializable]
    public class BlockingTile
    {
        public TileBase Tile;
        [Tooltip("determines which levels get blocked, structures that occupy the same level cannot be built on the tile")]
        public StructureLevelMask Level;
    }
}
