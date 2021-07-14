namespace CityBuilderCore
{
    public enum StorageOrderMode
    {
        /// <summary>
        /// passively accept item up to ratio
        /// </summary>
        Neutral = 0,
        /// <summary>
        /// actively get item up to ratio
        /// </summary>
        Get = 10,
        /// <summary>
        /// actively get rid of item down to ratio
        /// </summary>
        Empty = 20
    }
}