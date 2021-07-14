using System;

namespace CityBuilderCore
{
    [Serializable]
    public class PopulationHousing
    {
        public Population Population;
        public int Capacity;
        public int Quantity { get; set; }
        public int Reserved { get; set; }

        public int GetRemainingCapacity()
        {
            return Capacity - Quantity - Reserved;
        }

        #region Saving
        [Serializable]
        public class PopulationHousingData
        {
            public int Quantity;
            public int Reserved;
        }

        public PopulationHousingData SaveData()
        {
            return new PopulationHousingData()
            {
                Quantity = Quantity,
                Reserved = Reserved
            };
        }
        public void LoadData(PopulationHousingData data)
        {
            Quantity = data.Quantity;
            Reserved = data.Reserved;
        }
        #endregion
    }
}