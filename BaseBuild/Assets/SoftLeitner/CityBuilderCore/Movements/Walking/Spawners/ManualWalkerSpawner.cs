using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// walker spawner that does not spawn on its own
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ManualWalkerSpawner<T> : WalkerSpawner<T>
    where T : Walker
    {
        public void Spawn(Action<T> onSpawned = null, Vector2Int? start = null)
        {
            spawn(onSpawned, start);
        }

        #region Saving
        public ManualWalkerSpawnerData SaveData()
        {
            return new ManualWalkerSpawnerData()
            {
                Walkers = _currentWalkers.Select(w => w.SaveData()).ToArray()
            };
        }
        public void LoadData(ManualWalkerSpawnerData data)
        {
            clearWalkers();

            foreach (var active in data.Walkers)
            {
                reloadActive().LoadData(active);
            }
        }
    }
    [Serializable]
    public class ManualWalkerSpawnerData
    {
        public string[] Walkers;
    }
    #endregion
}