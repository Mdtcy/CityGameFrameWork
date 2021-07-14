using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    public class ConnectionPasserTiles : ConnectionPasserBase
    {
        public Tilemap Tilemap;
        public TileBase Tile;

        private List<Vector2Int> _points = new List<Vector2Int>();

        protected override void Start()
        {
            foreach (var position in Tilemap.cellBounds.allPositionsWithin)
            {
                if (Tile == null && Tilemap.HasTile(position) || Tilemap.GetTile(position) == Tile)
                {
                    _points.Add((Vector2Int)position);
                }
            }

            base.Start();
        }

        public override IEnumerable<Vector2Int> GetPoints() => _points;
    }
}
