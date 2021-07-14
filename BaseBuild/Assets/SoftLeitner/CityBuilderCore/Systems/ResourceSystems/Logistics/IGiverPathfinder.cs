using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// calculates the best path for getting a needed item
    /// </summary>
    public interface IGiverPathfinder
    {
        BuildingComponentPath<IItemGiver> GetGiverPath(IBuilding building, Vector2Int? currentPoint, ItemQuantity items, float maxDistance, PathType pathType, object pathTag = null);
    }
}