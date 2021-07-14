using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// type of population(plebs, middle class, snobs, ...)<br/>
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(Population))]
    public class Population : KeyedObject
    {
        public string Name;
    }
}