using System;

namespace CityBuilderCore
{
    public class WalkingState
    {
        public WalkingPath Path;
        public float Moved;
        public int Index;

        #region Saving
        [Serializable]
        public class WalkingData
        {
            public WalkingPath.WalkingPathData WalkingPathData;
            public float Delay;
            public float Moved;
            public int Index;
        }

        public WalkingData GetData() => new WalkingData()
        {
            WalkingPathData = Path.GetData(),
            Moved = Moved,
            Index = Index
        };
        public static WalkingState FromData(WalkingData data)
        {
            if (data == null)
                return null;
            return new WalkingState()
            {
                Path = WalkingPath.FromData(data.WalkingPathData),
                Moved = data.Moved,
                Index = data.Index
            };
        }
        #endregion
    }
}