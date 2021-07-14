using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public interface ILayerManager
    {
        /// <summary>
        /// fired whenever an affector is added or removed
        /// </summary>
        event Action<Layer> Changed;

        /// <summary>
        /// register affector with manager, layer values are recomputed at registration so if value inside affector change it has to be re-registered
        /// </summary>
        /// <param name="affector"></param>
        void Register(ILayerAffector affector);
        void Deregister(ILayerAffector affector);

        /// <summary>
        /// checks if the points in the specified box, overall, satisfy the requirements
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="size"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
        bool CheckRequirement(Vector2Int origin, Vector2Int size, LayerRequirement requirement);
        /// <summary>
        /// returns a layer dependency if one exists at that point<br/>
        /// dependencies might be evolutions, roads, ...
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        IEnumerable<ILayerDependency> GetDependencies(Vector2Int position);

        /// <summary>
        /// returns an explanation of the layer value at a certain point(basevalue+affectors)
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        LayerKey GetKey(Layer layer, Vector2Int position);
        /// <summary>
        /// returns the computed total value of a layer at a point
        /// </summary>
        /// <param name="position"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        int GetValue(Vector2Int position, Layer layer);
        /// <summary>
        /// returns all positions and values of a layer where the value differs from 0
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        IEnumerable<Tuple<Vector2Int, int>> GetValues(Layer layer);
    }
}