using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// a risk that when executed replaces the building with structures(eg Rubble)
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Risks/" + nameof(RiskStructureReplacement))]
    public class RiskStructureReplacement : Risk
    {
        public string StructureCollectionKey;

        public override void Execute(IRiskRecipient risker)
        {
            var collection = Dependencies.Get<IStructureManager>().GetStructureCollection(StructureCollectionKey);
            var positions = PositionHelper.GetBoxPositions(risker.Building.Point, risker.Building.Point + risker.Building.Size - Vector2Int.one, collection.ObjectSize);

            risker.Building.Terminate();
            collection.Add(positions);
        }
        public override void Resolve(IRiskRecipient risker)
        {

        }
    }
}