using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// provides workery periodically as long as the buildings efficiency is working
    /// </summary>
    public class CyclicWorkerProviderComponent : BuildingComponent
    {
        public override string Key => "CWP";

        public CyclicWorkerWalkerSpawner WorkerWalkers;

        private void Awake()
        {
            WorkerWalkers.Initialize(Building, workerLeaving);
        }

        private void Update()
        {
            if (Building.IsWorking)
                WorkerWalkers.Update();
        }

        protected bool workerLeaving(WorkerWalker walker)
        {
            var workerPath = Dependencies.Get<IWorkplaceFinder>().GetWorkerPath(Building, null, walker.Worker, walker.Storage, walker.MaxDistance, walker.PathType, walker.PathTag);
            if (workerPath == null)
                return false;
            walker.StartWorking(workerPath);
            return true;
        }

        #region Saving
        [Serializable]
        public class CyclicWorkerProviderData
        {
            public CyclicWalkerSpawnerData WorkerWalkers;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new CyclicWorkerProviderData()
            {
                WorkerWalkers = WorkerWalkers.SaveData()
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<CyclicWorkerProviderData>(json);

            WorkerWalkers.LoadData(data.WorkerWalkers);
        }
        #endregion
    }
}