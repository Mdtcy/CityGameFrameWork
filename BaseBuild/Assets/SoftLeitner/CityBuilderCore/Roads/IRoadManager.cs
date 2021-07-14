using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public interface IRoadManager : ISaveData
    {
        bool HasPoint(Vector2Int point, Road road = null);
        void Add(IEnumerable<Vector2Int> points, Road road);
        void Register(IEnumerable<Vector2Int> points, Road road);
        void Deregister(IEnumerable<Vector2Int> points, Road road);
        void RegisterSwitch(Vector2Int point, Road roadA, Road roadB);
        void RegisterSwitch(Vector2Int entry, Vector2Int point, Vector2Int exit, Road roadEntry, Road roadExit);
        bool CheckRequirement(Vector2Int point, RoadRequirement requirement);
        void Block(IEnumerable<Vector2Int> points, Road road = null);
        void Unblock(IEnumerable<Vector2Int> points, Road road = null);
    }
}