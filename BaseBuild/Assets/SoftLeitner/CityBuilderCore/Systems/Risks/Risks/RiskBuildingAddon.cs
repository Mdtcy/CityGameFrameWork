using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// a risk that when executed adds an addon to its building<br/>
    /// eg Fire, Disease, ...
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Risks/" + nameof(RiskBuildingAddon))]
    public class RiskBuildingAddon : Risk
    {
        public BuildingAddon Prefab;
        [Tooltip("whether the risk should be removed if the risk gets resolved(eg a doctor resolving a disease before the mortality gets triggered)")]
        public bool Remove;

        public override void Execute(IRiskRecipient risker)
        {
            risker.Building.AddAddon(Prefab);
        }

        public override void Resolve(IRiskRecipient risker)
        {
            if (Remove)
                risker.Building.RemoveAddon(Prefab.Key);
        }
    }
}