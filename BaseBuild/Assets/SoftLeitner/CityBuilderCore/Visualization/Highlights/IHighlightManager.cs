using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    /// <summary>
    /// highlights tiles as valid, invalid or just as info<br/>
    /// primarily used by building tools
    /// </summary>
    public interface IHighlightManager
    {
        void Clear();
        void Highlight(IEnumerable<Vector2Int> positions, bool valid);
        void Highlight(IEnumerable<Vector2Int> positions, HighlightType type);
        void Highlight(IEnumerable<Vector2Int> positions, TileBase tile);
        void Highlight(Vector2Int position, bool isValid);
        void Highlight(Vector2Int position, HighlightType type);
        void Highlight(Vector2Int position, TileBase tile);
    }
}