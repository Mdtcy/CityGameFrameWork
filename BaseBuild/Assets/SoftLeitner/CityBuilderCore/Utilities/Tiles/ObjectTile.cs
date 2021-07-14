using UnityEngine;
using UnityEngine.Tilemaps;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/Tiles/" + nameof(ObjectTile))]
    public class ObjectTile : TileBase
    {
        public GameObject Prefab;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);

            tileData.gameObject = Prefab;
        }
    }
}
