using System;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class RoadRequirement
    {
        public Vector2Int Point;
        public Road Road;
        [Tooltip("key the target road stage has to start with")]
        public string Stage;
        [Tooltip("should the road be added by the builder if its missing")]
        public bool Amend;

        public bool Check(Vector2Int point, Road road, string stage)
        {
            if (road == null)
            {
                if (!Amend)
                    return false;

                if (Road)
                    return Dependencies.Get<IStructureManager>().CheckAvailability(point, Road.Level.Value);
                else
                    return true;
            }
            else
            {
                if (Road && Road != road)
                    return false;

                if (!string.IsNullOrEmpty(Stage) && !stage.StartsWith(Stage))
                    return false;

                return true;
            }
        }
    }
}