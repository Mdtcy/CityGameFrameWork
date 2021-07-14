using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// manages attack system
    /// <para>
    /// attackers attack attackableComponents and get fended off by defenderComponents<br/>
    /// all entities with health get healthbars
    /// </para>
    /// </summary>
    public interface IAttackManager
    {
        void AddAttacker(IAttacker attacker);
        void RemoveAttacker(IAttacker attacker);

        void AddHealther(IHealther healther);
        void RemoveHealther(IHealther healther);

        IAttacker GetAttacker(Vector3 position, float maxDistance);
        BuildingComponentPath<IAttackable> GetAttackerPath(Vector2Int point, PathType pathType, object tag = null);
    }
}