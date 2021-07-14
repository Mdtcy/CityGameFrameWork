using UnityEngine;

namespace CityBuilderCore
{
    public interface IMouseInput
    {
        Ray GetRay();
        Vector3 GetMousePosition();
        Vector2 GetMouseScreenPosition();
        Vector2Int GetMouseGridPosition();
    }
}