using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    [Serializable]
    public class LayerAffectorTile
    {
        public TileBase Tile;
        public Layer Layer;
        [Tooltip("value inside the affector")]
        public int Value;
        [Tooltip("range of points outside the affector")]
        public int Range;
        [Tooltip("value subtracted for every step outside the affector")]
        public int Falloff;

        private List<Vector2Int> _positions = new List<Vector2Int>();

        public void AddPosition(Vector2Int position)
        {
            _positions.Add(position);
        }

        public int GetValue(Vector2Int position)
        {
            if (_positions.Contains(position))
                return Value;
            var distance = _positions.Min(p => p.GetMaxAxisDistance(position));
            if (distance > Range)
                return 0;
            else
                return Value - distance * Falloff;
        }
    }
}