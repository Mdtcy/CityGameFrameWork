using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// calculates the best path for depositing an item(eg items produced in production)
    /// </summary>
    public interface IReceiverPathfinder
    {
        BuildingComponentPath<IItemReceiver> GetReceiverPath(IBuilding building, Vector2Int? currentPoint, ItemQuantity items, float maxDistance, PathType pathType, object pathTag = null, int currentPriority = 0);
    }
}