using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public class RoamingState
    {
        public int Steps;
        public float Moved;
        public Vector2Int Current;
        public Vector2Int Next;
        public List<Vector2Int> Memory = new List<Vector2Int>();

        #region Saving
        [Serializable]
        public class RoamingData
        {
            public int Steps;
            public float Moved;
            public Vector2Int Current;
            public Vector2Int Next;
            public List<Vector2Int> Memory;
        }

        public RoamingData GetData() => new RoamingData()
        {
            Steps = Steps,
            Moved = Moved,
            Current = Current,
            Next = Next,
            Memory = Memory
        };
        public static RoamingState FromData(RoamingData data)
        {
            if (data == null)
                return null;
            return new RoamingState()
            {
                Steps = data.Steps,
                Moved = data.Moved,
                Current = data.Current,
                Next = data.Next,
                Memory = data.Memory
            };
        }
        #endregion
    }
}