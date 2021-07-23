/**
 * @author BoLuo
 * @email [ tktetb@163.com ]
 * @create date 23:47:34
 * @modify date 23:47:34
 * @desc []
 */

using System.Collections.Generic;
using UnityEngine;

namespace Build.Structure
{
    public interface IStructureManager
    {
        /// <summary>
        /// checks if a point is available for building(not blocked, nothing in the way)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="excludeRoads">true for structures that can be built on top of roads</param>
        /// <returns></returns>
        bool CheckAvailability(Vector2Int point, int mask);

        /// <summary>
        /// checks if a point already has a structure
        /// </summary>
        /// <param name="point"></param>
        /// <param name="excludeRoads">true is roads can be ignored</param>
        /// <returns></returns>
        bool HasStructure(Vector2Int point, int mask);

        IEnumerable<IStructure> GetStructures(Vector2Int point, int mask);

        void RegisterStructure(IStructure structure, bool isUnderlying = false);
        void DeregisterStructure(IStructure structure, bool isUnderlying = false);

    }
}