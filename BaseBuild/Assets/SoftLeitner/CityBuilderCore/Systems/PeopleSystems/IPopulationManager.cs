using System.Collections.Generic;

namespace CityBuilderCore
{
    /// <summary>
    /// manages the population and housing system<br/>
    /// <para>
    /// keeps track of different population migrations, housings and statistics
    /// </para>
    /// </summary>
    public interface IPopulationManager
    {
        Migration GetMigration(Population population);

        IEnumerable<IHousing> GetHousings();
        int GetQuantity(Population population, bool includeReserved = false);
        int GetRemainingCapacity(Population population);

        void AddHomeless(Population population, IHousing housing, int quantity);

        string SaveData();
        void LoadData(string json);
    }
}