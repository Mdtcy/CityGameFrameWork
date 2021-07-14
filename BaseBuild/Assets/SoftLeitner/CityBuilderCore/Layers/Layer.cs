using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// layers are arrays of numbers underlying the map points(desirability, fertility, resources)
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(Layer))]
    public class Layer : ScriptableObject
    {
        [Tooltip("display name")]
        public string Name;
        [Tooltip("when two affectors affect the same position cumulative layers sum them while others use the bigger value")]
        public bool IsCumulative = true;
    }
}