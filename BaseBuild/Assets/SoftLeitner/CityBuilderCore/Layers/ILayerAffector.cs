using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// an object affecting the layer values, is registered with <see cref="ILayerManager"/>
    /// </summary>
    public interface ILayerAffector
    {
        Layer Layer { get; }
        string Name { get; }

        event Action<ILayerAffector> Changed;

        /// <summary>
        /// return all positions the affector affects with their values
        /// </summary>
        /// <returns></returns>
        Dictionary<Vector2Int, int> GetValues();
    }
}