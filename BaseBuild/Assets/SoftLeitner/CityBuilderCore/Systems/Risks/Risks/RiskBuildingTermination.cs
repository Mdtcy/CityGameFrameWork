using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// a risk that when executed terminates the building
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Risks/" + nameof(RiskBuildingTermination))]
    public class RiskBuildingTermination : Risk
    {
        public string StructureCollectionKey;

        public override void Execute(IRiskRecipient risker)
        {
            risker.Building.Terminate();
        }
        public override void Resolve(IRiskRecipient risker)
        {

        }
    }
}