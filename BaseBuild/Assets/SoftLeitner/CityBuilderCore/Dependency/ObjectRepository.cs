using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// repository used to define and find the objects available to a scene
    /// </summary>
    public class ObjectRepository : MonoBehaviour
    {
        [Header("Sets")]
        public BuildingAddonSet Addons;
        public BuildingInfoSet Buildings;
        public ItemSet Items;
        public PopulationSet Populations;
        public RoadSet Roads;
        public ScoreSet Scores;
        public EmploymentGroupSet EmploymentGroups;

        [Header("Singles (convenience option when theres only one)")]
        public BuildingAddon Addon;
        public BuildingInfo Building;
        public Item Item;
        public Population Population;
        public Road Road;
        public Score Score;
        public EmploymentGroup EmploymentGroup;

        private void Awake()
        {
            Dependencies.Register(getKeyedSet(Addons, Addon));
            Dependencies.Register(getKeyedSet(Buildings, Building));
            Dependencies.Register(getKeyedSet(Items, Item));
            Dependencies.Register(getKeyedSet(Populations, Population));
            Dependencies.Register(getKeyedSet(EmploymentGroups, EmploymentGroup));

            Dependencies.Register(getObjectSet(Addons, Addon));
            Dependencies.Register(getObjectSet(Buildings, Building));
            Dependencies.Register(getObjectSet(Items, Item));
            Dependencies.Register(getObjectSet(Populations, Population));
            Dependencies.Register(getObjectSet(Roads, Road));
            Dependencies.Register(getObjectSet(Scores, Score));
            Dependencies.Register(getObjectSet(EmploymentGroups, EmploymentGroup));
        }

        private IKeyedSet<T> getKeyedSet<T>(IKeyedSet<T> set, T single) where T : IKeyed
        {
            if (set != null)
                return set;
            else if (single != null)
                return new RuntimeKeyedSet<T>(single);
            else
                return new RuntimeKeyedSet<T>();
        }

        private IObjectSet<T> getObjectSet<T>(IObjectSet<T> set, T single)
        {
            if (set != null)
                return set;
            else if (single != null)
                return new RuntimeObjectSet<T>(single);
            else
                return new RuntimeObjectSet<T>();
        }
    }
}