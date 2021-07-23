/**
 * @author BoLuo
 * @email [ tktetb@163.com ]
 * @create date 22:14:41
 * @modify date 22:14:41
 * @desc []
 */

using UnityEngine;

namespace Build.Map
{
    /// <summary>
    /// rotates transforms, 3d implementations might rotate 360° while isometric games may only mirror
    /// </summary>
    public interface IGridRotations
    {
        void SetRotation(Transform transform, Vector3 direction);
    }
}