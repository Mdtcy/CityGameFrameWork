namespace CityBuilderCore
{
    /// <summary>
    /// a building component that houses populations(a hut that provides housing for 20 plebs, a villa that provides housing for 5 snobs)
    /// </summary>
    public interface IHousing : IBuildingTrait<IHousing>
    {
        int GetQuantity(Population population, bool includeReserved = false);
        int GetRemainingCapacity(Population population);
        int Reserve(Population population, int quantity);
        int Inhabit(Population population, int quantity);
        int Abandon(Population population, int quantity);
        void Kill(float ratio);
    }
}