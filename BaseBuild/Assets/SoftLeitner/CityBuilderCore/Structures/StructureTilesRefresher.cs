using UnityEngine;

namespace CityBuilderCore
{
    public class StructureTilesRefresher : MonoBehaviour
    {
        public string Key;

        private void Start()
        {
            var tiles = Dependencies.Get<IStructureManager>().GetStructureTiles(Key);
            if (!tiles)
                return;

            var building = GetComponent<IBuilding>();

            if (building == null)
            {
                var gridPosition = Dependencies.Get<IGridPositions>().GetGridPosition(transform.position);

                tiles.RefreshTile(gridPosition);

                foreach (var point in PositionHelper.GetRing(gridPosition, 1))
                {
                    tiles.RefreshTile(point);
                }
            }
            else
            {
                foreach (var point in PositionHelper.GetBoxPositions(building.Point - Vector2Int.one, building.Point + building.Size + Vector2Int.one))
                {
                    tiles.RefreshTile(point);
                }
            }
        }
    }
}
