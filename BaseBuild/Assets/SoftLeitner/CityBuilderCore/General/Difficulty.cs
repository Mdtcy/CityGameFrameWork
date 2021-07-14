using UnityEngine;

namespace CityBuilderCore
{
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(Difficulty))]
    public class Difficulty : ScriptableObject, IDifficultyFactor
    {
        [Tooltip("key used in save/load")]
        public string Key;
        [Tooltip("display name")]
        public string Name;

        [Tooltip("influences the speed at which risks increase")]
        public float RiskMultiplier;
        [Tooltip("influences the speed at which services deplete")]
        public float ServiceMultiplier;
        [Tooltip("influences the speed at which items deplete")]
        public float ItemsMultiplier;

        float IDifficultyFactor.RiskMultiplier => RiskMultiplier;
        float IDifficultyFactor.ServiceMultiplier => ServiceMultiplier;
        float IDifficultyFactor.ItemsMultiplier => ItemsMultiplier;
    }
}