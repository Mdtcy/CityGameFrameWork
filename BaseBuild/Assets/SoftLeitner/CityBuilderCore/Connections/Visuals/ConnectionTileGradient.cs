using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    public class ConnectionTileGradient : MonoBehaviour
    {
        public ConnectionPasserBase ConnectionPasser;
        public Tilemap Tilemap;
        public Gradient Gradient;
        public int Maximum;

        private void Awake()
        {
            if (ConnectionPasser)
                ConnectionPasser.PointValueChanged.AddListener(new UnityAction<Vector2Int, int>(Apply));
        }

        public void Apply(Vector2Int point, int value)
        {
            Tilemap.SetTileFlags((Vector3Int)point, TileFlags.None);
            Tilemap.SetColor((Vector3Int)point, Gradient.Evaluate(value / (float)Maximum));
        }
    }
}
