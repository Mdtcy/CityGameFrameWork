using UnityEngine;

namespace CityBuilderCore
{
    public interface IGridHeights
    {
        void SetHeight(Transform transform, Vector3 position, PathType pathType = PathType.Map, float? overrideValue = null);
    }
}
