namespace CityBuilderCore
{
    /// <summary>
    /// base class for views that visualize a <see cref="IBuildingValue"/> with a bar
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ViewBar<T> : ViewBarBase where T : IBuildingValue
    {
        public T Value;

        public override IBuildingValue BuildingValue => Value;
    }
}