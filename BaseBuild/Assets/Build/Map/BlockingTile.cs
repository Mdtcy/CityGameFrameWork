/**
 * @author BoLuo
 * @email [ tktetb@163.com ]
 * @create date 22:15:18
 * @modify date 22:15:18
 * @desc []
 */

using System;
using Build.Structure;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Build.Map
{
    [Serializable]
    public class BlockingTile
    {
        public TileBase Tile;

        [Tooltip("determines which levels get blocked, structures that occupy the same level cannot be built on the tile")]
        public StructureLevelMask Level;
    }
}