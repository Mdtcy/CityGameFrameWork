using UnityEngine;

namespace CityBuilderCore
{
    public interface IConnectionManager
    {
        void Calculate();

        void Register(IConnectionPasser passer);
        void Deregister(IConnectionPasser passer);

        bool HasPoint(Connection connection, Vector2Int point);
        int GetValue(Connection connection, Vector2Int point);
    }
}