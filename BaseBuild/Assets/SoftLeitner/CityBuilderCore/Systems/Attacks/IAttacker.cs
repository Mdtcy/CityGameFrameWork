using UnityEngine;

namespace CityBuilderCore
{
    public interface IAttacker
    {
        Vector3 Position { get; }
        void Hurt(int damage);
    }
}