using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public abstract class ConnectionPasserBase : MonoBehaviour, IConnectionPasser
    {
        public Connection Connection;

        public PointValueEvent PointValueChanged;

        Connection IConnectionPasser.Connection => Connection;

        public event Action<PointsChanged<IConnectionPasser>> PointsChanged;

        protected virtual void Start()
        {
            Dependencies.Get<IConnectionManager>().Register(this);
        }

        protected virtual void OnDestroy()
        {
            if (gameObject.scene.isLoaded)
                Dependencies.Get<IConnectionManager>().Deregister(this);
        }

        public abstract IEnumerable<Vector2Int> GetPoints();

        public void ValueChanged(Vector2Int point, int value)
        {
            PointValueChanged?.Invoke(point, value);
        }

        protected void onPointsChanged(IEnumerable<Vector2Int> removed, IEnumerable<Vector2Int> added) => PointsChanged?.Invoke(new PointsChanged<IConnectionPasser>(this, removed, added));
    }
}
