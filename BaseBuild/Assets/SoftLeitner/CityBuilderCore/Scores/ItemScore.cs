using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// item quantity in global storage
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/Scores/" + nameof(ItemScore))]
    public class ItemScore : Score
    {
        public Item Item;

        public override int Calculate()
        {
            return Dependencies.Get<IGlobalStorage>().Items.GetItemQuantity(Item);
        }
    }
}