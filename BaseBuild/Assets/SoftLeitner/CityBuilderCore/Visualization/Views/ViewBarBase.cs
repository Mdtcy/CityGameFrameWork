namespace CityBuilderCore
{
    /// <summary>
    /// non generic base class so <see cref="ViewBar{T}"/> can be accessed in <see cref="IBarManager"/>
    /// </summary>
    public abstract class ViewBarBase : View
    {
        public BaseValueBar Bar;
        public abstract IBuildingValue BuildingValue { get; }

        public override void Activate() => Dependencies.Get<IBarManager>().ActivateBars(this);
        public override void Deactivate() => Dependencies.Get<IBarManager>().ClearBars();
    }
}