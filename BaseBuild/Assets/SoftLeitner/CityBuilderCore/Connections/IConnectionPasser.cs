using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public interface IConnectionPasser
    {
        Connection Connection { get; }

        event Action<PointsChanged<IConnectionPasser>> PointsChanged;

        IEnumerable<Vector2Int> GetPoints();
        void ValueChanged(Vector2Int point, int value);
    }
}
