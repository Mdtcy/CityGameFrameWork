using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// dispenses items to items retrievers<br/>
    /// (eg map resources like trees that dispense wood, animals that dispense meat, ...)
    /// </summary>
    public interface IItemsDispenser
    {
        string Key { get; }
        Vector3 Position { get; }
        ItemQuantity Dispense();
    }
}