using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// finds the best work and supply way for a worker
    /// </summary>
    public interface IWorkplaceFinder
    {
        WorkerPath GetWorkerPath(IBuilding structure, Vector2Int? currentPoint, Worker worker, ItemStorage storage, float maxDistance, PathType pathType, object pathTag);
    }
}