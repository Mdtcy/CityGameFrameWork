using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// keeps track of all the <see cref="IItemsDispenser"/> and calculates the dispenser a retriever goes for
    /// </summary>
    public interface IItemsDispenserManager
    {
        void Add(IItemsDispenser dispenser);
        void Remove(IItemsDispenser dispenser);

        IItemsDispenser GetDispenser(string key, Vector3 position, float maxDistance);
        bool HasDispenser(string key, Vector3 position, float maxDistance);
    }
}