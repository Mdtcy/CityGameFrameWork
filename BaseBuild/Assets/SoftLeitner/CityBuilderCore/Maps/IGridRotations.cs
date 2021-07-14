using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// rotates transforms, 3d implementations might rotate 360° while isometric games may only mirror
    /// </summary>
    public interface IGridRotations
    {
        void SetRotation(Transform transform, Vector3 direction);
    }
}