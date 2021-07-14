using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// addon that simulates a fire<br/>
    /// <para>
    /// burns for some time, then replaces the building with a defined structure(eg Rubble)<br/>
    /// also completely disrupts building efficiency for obvious reasons
    /// </para>
    /// </summary>
    public class FireAddon : BuildingAddon, IEfficiencyFactor
    {
        [Tooltip("key of the structure collection the building is replaced after the fire burns out")]
        public string StructureCollectionKey;
        [Tooltip("how long the fire takes to burn out")]
        public float Duration;

        public bool IsWorking => false;
        public float Factor => 0f;

        private float _progress;

        public override void Update()
        {
            base.Update();

            _progress += Time.deltaTime / Duration;
            if (_progress >= 1f)
            {
                var collection = Dependencies.Get<IStructureManager>().GetStructureCollection(StructureCollectionKey);
                var positions = PositionHelper.GetBoxPositions(Building.Point, Building.Point + Building.Size - Vector2Int.one, collection.ObjectSize);

                Building.Terminate();
                collection.Add(positions);
            }
        }

        #region Saving
        [Serializable]
        public class FireData
        {
            public float Progress;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new FireData()
            {
                Progress = _progress
            });
        }
        public override void LoadData(string json)
        {
            var data = JsonUtility.FromJson<FireData>(json);

            _progress = data.Progress;
        }
        #endregion
    }
}