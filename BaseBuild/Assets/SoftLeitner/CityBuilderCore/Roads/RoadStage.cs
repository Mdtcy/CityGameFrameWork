using System;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    [Serializable]
    public class RoadStage
    {
        public LayerRequirement[] LayerRequirements;

        public string Key;
        public TileBase Tile;
    }
}