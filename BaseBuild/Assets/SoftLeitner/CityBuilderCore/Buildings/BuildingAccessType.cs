namespace CityBuilderCore
{
    /// <summary>
    /// which points are used to access the building
    /// </summary>
    public enum BuildingAccessType
    {
        /// <summary>
        /// 建筑周围的任意点
        /// </summary>
        Any = 0,
        /// <summary>
        /// 特殊点
        /// </summary>
        Exclusive = 10,
        /// <summary>
        /// 如果有特殊点就用，没有就任意
        /// </summary>
        Preferred = 20
    }
}