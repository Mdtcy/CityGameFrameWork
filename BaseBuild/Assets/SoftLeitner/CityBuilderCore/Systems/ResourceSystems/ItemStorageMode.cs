namespace CityBuilderCore
{
    public enum ItemStorageMode
    {
        /// <summary>
        /// storage consists of several sub stacks
        /// </summary>
        Stacked,
        /// <summary>
        /// stores anything without limitations
        /// </summary>
        Free,
        /// <summary>
        /// stores anything up to a quantity per item
        /// </summary>
        FreeItemCapped,
        /// <summary>
        /// stores anything up to a unit amount per item
        /// </summary>
        FreeUnitCapped,
        /// <summary>
        /// storage acts as a proxy for <see cref="IGlobalStorage"/>
        /// </summary>
        Global
    }
}