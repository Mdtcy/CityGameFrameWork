using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// meta info for data that does not change between instances of a building<br/>
    /// can be used to compare buildings(is that building a silo?)
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(BuildingInfo))]
    public class BuildingInfo : KeyedObject
    {
        [Tooltip("display name")]
        public string Name;
        [Tooltip("display description")]
        [TextArea]
        public string Description;
        [Tooltip("size of the building on the grid")]
        public Vector2Int Size;
        [Tooltip("can the building be removed by regular means")]
        public bool IsDestructible = true;
        [Tooltip("whether grid walkers can traverse the points of this building")]
        public bool IsWalkable = false;
        [Tooltip("prefab used when the building is built")]
        public Building Prefab;
        [Tooltip("prefab used to display building during building")]
        public GameObject Ghost;
        [Tooltip("use any point to access building or a special AccessPoint")]
        public BuildingAccessType AccessType;
        [Tooltip("point used for access when AccessType is Preferred or Exclusive")]
        public Vector2Int AccessPoint;
        [Tooltip("Items to be subtracted from GlobalStorage for building")]
        public ItemQuantity[] Cost;
        [Tooltip("map based requirements for building can be either layer or map-ground based or both, just leave the one you dont need empty")]
        public BuildingRequirement[] BuildingRequirements;
        [Tooltip("road based requirements for building, specify which points of the building need what kind of road and whether the road is automatically added")]
        public RoadRequirement[] RoadRequirements;
        [Tooltip("determines which structures can reside in the same position")]
        public StructureLevelMask Level;

        public virtual bool CheckBuildingRequirements(Vector2Int point, BuildingRotation rotation)
        {
            if (BuildingRequirements.Any(r => !r.IsFulfilled(point, Size, rotation)))
                return false;
            if (RoadRequirements.Any(r => !Dependencies.Get<IRoadManager>().CheckRequirement(rotation.RotateBuildingPoint(point, r.Point, Size), r)))
                return false;
            return true;
        }
        public virtual bool CheckBuildingAvailability(Vector2Int point)
        {
            return Dependencies.Get<IStructureManager>().CheckAvailability(point, Level.Value);
        }

        public virtual void Prepare(Vector2Int point, BuildingRotation rotation)
        {
            RoadRequirements.Where(r => r.Amend && r.Road).ForEach(r => Dependencies.Get<IRoadManager>().Add(new[] { rotation.RotateBuildingPoint(point, r.Point, Size) }, r.Road));
        }
    }
}