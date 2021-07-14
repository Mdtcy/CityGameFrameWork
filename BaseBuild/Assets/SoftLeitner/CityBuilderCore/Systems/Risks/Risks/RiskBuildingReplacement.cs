using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// a risk that when executed replaces its building with something else<br/>
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Risks/" + nameof(RiskBuildingReplacement))]
    public class RiskBuildingReplacement : Risk
    {
        public Building Replacement;

        public override void Execute(IRiskRecipient risker)
        {
            risker.Building.Replace(Replacement);
        }
        public override void Resolve(IRiskRecipient risker)
        {

        }
    }
}