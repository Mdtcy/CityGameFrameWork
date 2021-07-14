namespace CityBuilderCore
{
    /// <summary>
    /// responsible for adding adding and removing visuals for building values
    /// </summary>
    public interface IBarManager
    {
        void ActivateBars(ViewBarBase viewBar);
        void ClearBars();
    }
}