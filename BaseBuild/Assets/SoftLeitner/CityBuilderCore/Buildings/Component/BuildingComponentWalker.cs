namespace CityBuilderCore
{
    /// <summary>
    /// base class for walkers that roam and perform actions when passing a certain type of building component
    /// </summary>
    public class BuildingComponentWalker<T> : BuildingWalker
    where T : class, IBuildingComponent
    {
        protected override void onEntered(IBuilding building)
        {
            base.onEntered(building);

            building.GetBuildingComponents<T>().ForEach(c => onComponentEntered(c));
        }
        protected override void onRemaining(IBuilding building)
        {
            base.onRemaining(building);

            building.GetBuildingComponents<T>().ForEach(c => onComponentRemaining(c));
        }

        protected virtual void onComponentEntered(T buildingComponent)
        {

        }
        protected virtual void onComponentRemaining(T buildingComponent)
        {

        }
    }
}