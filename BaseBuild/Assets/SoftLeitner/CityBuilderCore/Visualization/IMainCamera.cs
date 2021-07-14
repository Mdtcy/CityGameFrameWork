using UnityEngine;

namespace CityBuilderCore
{
    public interface IMainCamera
    {
        Camera Camera { get; }
        Vector3 Position { get; set; }
        float Size { get; set; }

        void SetCulling(LayerMask layerMask);
        void ResetCulling();
    }
}