using System;

namespace CityBuilderCore
{
    [Serializable]
    public class BuildingEvaluation
    {
        public BuildingInfo Building;
        public int Value;

        public int GetCount() => Dependencies.Get<IBuildingManager>().Count(Building);
        public int GetValue() => Value;
        public int GetEvaluation() => GetCount() * GetValue();
    }
}