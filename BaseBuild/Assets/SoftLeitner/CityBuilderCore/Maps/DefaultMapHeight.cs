using UnityEngine;

namespace CityBuilderCore
{
    public class DefaultMapHeight : MonoBehaviour, IGridHeights
    {
        public Terrain Terrain;
        public float RoadHeight;
        public float MapHeight;

        private IMap _map;

        private void Awake()
        {
            Dependencies.Register<IGridHeights>(this);
        }

        private void Start()
        {
            _map = Dependencies.Get<IMap>();
        }

        public void SetHeight(Transform transform, Vector3 position, PathType pathType = PathType.Map, float? overrideValue = null)
        {
            float height;

            if (overrideValue.HasValue)
            {
                height = overrideValue.Value;
            }
            else
            {
                switch (pathType)
                {
                    case PathType.Road:
                    case PathType.RoadBlocked:
                        height = RoadHeight;
                        break;
                    default:
                        height = MapHeight;
                        if (Terrain)
                            height += Terrain.SampleHeight(position);
                        break;
                }
            }

            if (_map.IsXY)
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, height);
            else
                transform.localPosition = new Vector3(transform.localPosition.x, height, transform.localPosition.z);
        }
    }
}
