using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    /// <summary>
    /// default implementation for <see cref="IHighlightManager"/><br/>
    /// uses tiles that are assigned in inspector
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    public class DefaultHighlightManager : MonoBehaviour, IHighlightManager
    {
        public TileBase ValidTile;
        public TileBase InvalidTile;
        public TileBase InfoTile;

        private Tilemap _tilemap;

        protected virtual void Awake()
        {
            Dependencies.Register<IHighlightManager>(this);

            _tilemap = GetComponent<Tilemap>();
        }

        public void Highlight(IEnumerable<Vector2Int> positions, bool valid) => Highlight(positions, valid ? ValidTile : InvalidTile);
        public void Highlight(IEnumerable<Vector2Int> positions, HighlightType type) => Highlight(positions, getTile(type));
        public void Highlight(IEnumerable<Vector2Int> positions, TileBase tile)
        {
            foreach (var position in positions)
            {
                _tilemap.SetTile((Vector3Int)position, tile);
            }
        }

        public void Highlight(Vector2Int position, bool isValid) => Highlight(position, isValid ? ValidTile : InvalidTile);
        public void Highlight(Vector2Int position, HighlightType type) => Highlight(position, getTile(type));
        public void Highlight(Vector2Int position, TileBase tile)
        {
            _tilemap.SetTile((Vector3Int)position, tile);
        }

        private TileBase getTile(HighlightType type)
        {
            switch (type)
            {
                case HighlightType.Valid:
                    return ValidTile;
                case HighlightType.Invalid:
                    return InvalidTile;
                case HighlightType.Info:
                    return InfoTile;
                default:
                    return null;
            }
        }

        public void Clear()
        {
            _tilemap.ClearAllTiles();
        }
    }
}