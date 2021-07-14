using System.Collections.Generic;

namespace CityBuilderCore
{
    /// <summary>
    /// building component that receives items passively<br/>
    /// for example from a <see cref="SaleWalker"/> walking by
    /// </summary>
    public interface IItemRecipient : IBuildingComponent
    {
        ItemRecipient[] ItemsRecipients { get; }
        ItemCategoryRecipient[] ItemsCategoryRecipients { get; }

        IEnumerable<ItemQuantity> GetItems();
    }
}