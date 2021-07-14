using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// building component that periodically spawns a <see cref="RiskWalker"/>
    /// </summary>
    public class WalkerComponent : BuildingComponent
    {
        public override string Key => "WLK";

        public CyclicRoamingWalkerSpawner CyclicRoamingWalkers;

        private void Awake()
        {
            CyclicRoamingWalkers.Initialize(Building);
        }
        private void Update()
        {
            if (Building.IsWorking)
                CyclicRoamingWalkers.Update();
        }

        #region Saving
        [Serializable]
        public class RiskWalkerData
        {
            public CyclicWalkerSpawnerData SpawnerData;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new RiskWalkerData() { SpawnerData = CyclicRoamingWalkers.SaveData() });
        }
        public override void LoadData(string json)
        {
            CyclicRoamingWalkers.LoadData(JsonUtility.FromJson<RiskWalkerData>(json).SpawnerData);
        }
        #endregion
    }
}