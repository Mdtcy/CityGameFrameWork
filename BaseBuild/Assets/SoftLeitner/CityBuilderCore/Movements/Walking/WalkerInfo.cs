using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// meta info for data that does not change between instances of a walker<br/>
    /// can be used to compare walkers
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(WalkerInfo))]
    public class WalkerInfo : ScriptableObject
    {
        [Tooltip("display name")]
        public string Name;
        [TextArea]
        [Tooltip("display descriptions")]
        public string[] Descriptions;
    }
}