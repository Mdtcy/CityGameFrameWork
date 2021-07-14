using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// category for bundling and filtering buildings(entertainment, religion, ....), mainly used in scores
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(BuildingCategory))]
    public class BuildingCategory : ScriptableObject
    {
        public string Key;
        public string NameSingular;
        public string NamePlural;
        public BuildingInfo[] Buildings;
    }
}