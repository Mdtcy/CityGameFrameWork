using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// some object that depends on layer values and has to be notified when they change
    /// </summary>
    public interface ILayerDependency
    {
        void CheckLayers(IEnumerable<Vector2Int> points);
    }
}