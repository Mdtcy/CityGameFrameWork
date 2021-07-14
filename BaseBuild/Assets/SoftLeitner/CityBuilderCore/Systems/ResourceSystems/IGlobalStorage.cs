namespace CityBuilderCore
{
    /// <summary>
    /// global storage for items<br/>
    /// used for building costs<br/>
    /// filled by buildings with <see cref="ItemStorageMode.Global"/>
    /// </summary>
    public interface IGlobalStorage
    {
        ItemStorage Items { get; }
    }
}