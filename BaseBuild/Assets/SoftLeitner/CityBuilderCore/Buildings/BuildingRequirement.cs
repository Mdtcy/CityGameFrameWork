using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    [System.Serializable]
    public class BuildingRequirement
    {
        [Tooltip("how the points a building might be built on are checked for its requirements")]
        public BuildingRequirementMode Mode;
        [Tooltip("the points being checked when using SPECIFIC mode")]
        public Vector2Int[] Points;
        [Tooltip("the minimum number of points when using ANY mode")]
        public int Count = 1;
        [Tooltip("a window of layer values that has to be fulfilled to be able to build(set layer to NONE to ignore)")]
        public LayerRequirement LayerRequirement;
        [Tooltip("an object the maps ground has to exhibit to be able to build(TILES when using the included maps)")]
        public Object[] GroundOptions;

        public bool IsFulfilled(Vector2Int point, Vector2Int size, BuildingRotation rotation, IEnumerable<Vector2Int> points = null)
        {
            if (LayerRequirement != null && LayerRequirement.Layer)
            {
                switch (Mode)
                {
                    case BuildingRequirementMode.Any:
                        if (!checkLayerAny(point, size, LayerRequirement, Count))
                            return false;
                        break;
                    case BuildingRequirementMode.Average:
                        if (!checkLayerAverage(point, size, LayerRequirement))
                            return false;
                        break;
                    case BuildingRequirementMode.Specific:
                        if (!checkLayerSpecific(point, size, rotation, LayerRequirement, points ?? Points))
                            return false;
                        break;
                    case BuildingRequirementMode.All:
                        if (!checkLayerAll(point, size, LayerRequirement))
                            return false;
                        break;
                }
            }

            if (GroundOptions != null && GroundOptions.Length > 0)
            {
                switch (Mode)
                {
                    case BuildingRequirementMode.Any:
                        if (!checkGroundAny(point, size, GroundOptions, Count))
                            return false;
                        break;
                    case BuildingRequirementMode.Average:
                        if (!checkGroundAverage(point, size, GroundOptions))
                            return false;
                        break;
                    case BuildingRequirementMode.Specific:
                        if (!checkGroundSpecific(point, size, rotation, GroundOptions, points ?? Points))
                            return false;
                        break;
                    case BuildingRequirementMode.All:
                        if (!checkGroundAll(point, size, GroundOptions))
                            return false;
                        break;
                }
            }

            return true;
        }

        private static bool checkLayerAny(Vector2Int point, Vector2Int size, LayerRequirement layerRequirement, int count)
        {
            var fulfilledPoints = 0;
            foreach (var structurePoint in PositionHelper.GetStructurePositions(point, size))
            {
                if (layerRequirement.IsFulfilled(structurePoint))
                    fulfilledPoints++;
                if (fulfilledPoints >= count)
                    return true;
            }
            return false;
        }
        private static bool checkLayerAverage(Vector2Int point, Vector2Int size, LayerRequirement layerRequirement)
        {
            var sum = 0f;
            foreach (var structurePoint in PositionHelper.GetStructurePositions(point, size))
            {
                sum += layerRequirement.GetValue(structurePoint);
            }
            return layerRequirement.CheckValue(Mathf.RoundToInt(sum / (size.x * size.y)));
        }
        private static bool checkLayerSpecific(Vector2Int point, Vector2Int size, BuildingRotation rotation, LayerRequirement layerRequirement, IEnumerable<Vector2Int> points)
        {
            foreach (var structurePoint in points.Select(p => rotation.RotateBuildingPoint(point, p, size)))
            {
                if (!layerRequirement.IsFulfilled(structurePoint))
                    return false;
            }
            return true;
        }
        private static bool checkLayerAll(Vector2Int point, Vector2Int size, LayerRequirement layerRequirement)
        {
            foreach (var structurePoint in PositionHelper.GetStructurePositions(point, size))
            {
                if (!layerRequirement.IsFulfilled(structurePoint))
                    return false;
            }
            return true;
        }

        private static bool checkGroundAny(Vector2Int point, Vector2Int size, Object[] groundOptions, int count)
        {
            var map = Dependencies.Get<IMap>();
            var fulfilledPoints = 0;
            foreach (var structurePoint in PositionHelper.GetStructurePositions(point, size))
            {
                if (map.CheckGround(structurePoint, groundOptions))
                    fulfilledPoints++;
                if (fulfilledPoints >= count)
                    return true;
            }
            return false;
        }
        private static bool checkGroundAverage(Vector2Int point, Vector2Int size, Object[] groundOptions)
        {
            var map = Dependencies.Get<IMap>();
            var sum = 0f;
            foreach (var structurePoint in PositionHelper.GetStructurePositions(point, size))
            {
                if (map.CheckGround(structurePoint, groundOptions))
                    sum += 1f;
            }
            return sum / (size.x * size.y) >= 0.5f;
        }
        private static bool checkGroundSpecific(Vector2Int point, Vector2Int size, BuildingRotation rotation, Object[] groundOptions, IEnumerable<Vector2Int> points)
        {
            var map = Dependencies.Get<IMap>();
            foreach (var structurePoint in points.Select(p => rotation.RotateBuildingPoint(point, p, size)))
            {
                if (!map.CheckGround(structurePoint, groundOptions))
                    return false;
            }
            return true;
        }
        private static bool checkGroundAll(Vector2Int point, Vector2Int size, Object[] groundOptions)
        {
            var map = Dependencies.Get<IMap>();
            foreach (var structurePoint in PositionHelper.GetStructurePositions(point, size))
            {
                if (!map.CheckGround(structurePoint, groundOptions))
                    return false;
            }
            return true;
        }
    }
}
