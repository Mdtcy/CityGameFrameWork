using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/Tiles/" + nameof(ConnectionRectangleTile))]
    public class ConnectionRectangleTile : ConnectedRectangleTile
    {
        public Connection Connection;

        protected override bool hasConnection(ITilemap tilemap, Vector3Int point)
        {
            if (base.hasConnection(tilemap, point))
                return true;

            if (Application.isPlaying)
                return Dependencies.Get<IConnectionManager>().HasPoint(Connection, (Vector2Int)point);
            else
                return false;
        }
    }
}
