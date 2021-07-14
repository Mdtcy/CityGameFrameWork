using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// shows camera view area on minimap
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class CameraArea : MonoBehaviour
    {
        public Camera Camera;

        private LineRenderer _lineRenderer;
        private Vector3[] _positions;
        private IMap _map;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _positions = new Vector3[4];
        }

        private void Start()
        {
            _map = Dependencies.Get<IMap>();
        }

        private void Update()
        {
            Ray topLeft = Camera.main.ViewportPointToRay(new Vector3(0, 0, 0));
            Ray topRight = Camera.main.ViewportPointToRay(new Vector3(1, 0, 0));
            Ray botRight = Camera.main.ViewportPointToRay(new Vector3(1, 1, 0));
            Ray botLeft = Camera.main.ViewportPointToRay(new Vector3(0, 1, 0));

            Plane plane;

            if (_map.IsXY)
                plane = new Plane(Vector3.forward, 0);
            else
                plane = new Plane(Vector3.up, 0);

            plane.Raycast(topLeft, out float topLeftEnter);
            plane.Raycast(topRight, out float topRightEnter);
            plane.Raycast(botRight, out float botRightEnter);
            plane.Raycast(botLeft, out float botLeftEnter);

            _positions[0] = topLeft.GetPoint(topLeftEnter);
            _positions[1] = topRight.GetPoint(topRightEnter);
            _positions[2] = botRight.GetPoint(botRightEnter < 0 ? 1000 : botRightEnter);
            _positions[3] = botLeft.GetPoint(Mathf.Abs(botLeftEnter < 0 ? 1000 : botLeftEnter));

            _lineRenderer.SetPositions(_positions);
        }
    }
}