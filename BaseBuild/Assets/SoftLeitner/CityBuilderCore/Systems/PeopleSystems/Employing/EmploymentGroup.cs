using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// types of employment that can have different priorities so less essential groups loose access first(services, logistics, food, industry, ...)
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(EmploymentGroup))]
    public class EmploymentGroup : KeyedObject
    {
        public string Name;
        public int Priority;
    }
}