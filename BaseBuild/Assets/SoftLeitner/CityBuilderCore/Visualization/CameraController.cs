using UnityEngine;
using UnityEngine.EventSystems;

namespace CityBuilderCore
{
    public class CameraController : MonoBehaviour, IMouseInput, IMainCamera
    {
        public Camera Camera;
        public Transform Pivot;

        public float Speed = 5;
        public float RotateSpeed = 50;
        public float PitchSpeed = 50;
        public float ZoomSpeed = 10;
        public float MinZoom = 2;
        public float MaxZoom = 15;
        public Vector3 SortAxis;

        private Camera _camera;
        private Transform _pivot;
        private int _defaultCulling;
        private Vector3 _previousMousePosition;
        private IMap _map;

        public Vector3 Position { get => _pivot.position; set => _pivot.position = value; }
        public float Size
        {
            get
            {
                if (_camera.orthographic)
                    return _camera.orthographicSize;
                else
                    return -_camera.transform.localPosition.z;
            }
            set
            {
                if (_camera.orthographic)
                    _camera.orthographicSize = value;
                else
                    _camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _camera.transform.localPosition.y, -value);
            }
        }

        Camera IMainCamera.Camera => _camera;

        protected virtual void Awake()
        {
            Dependencies.Register<IMouseInput>(this);
            Dependencies.Register<IMainCamera>(this);
        }

        protected virtual void Start()
        {
            _map = Dependencies.Get<IMap>();

            if (Camera)
                _camera = Camera;
            else
                _camera = GetComponent<Camera>();

            if (Pivot)
                _pivot = Pivot;
            else
                _pivot = transform;

            _defaultCulling = _camera.cullingMask;

            if (SortAxis.sqrMagnitude > 0)
            {
                _camera.transparencySortMode = TransparencySortMode.CustomAxis;
                _camera.transparencySortAxis = SortAxis;
            }
        }

        void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            Vector3 position = _pivot.position;
            var deltaTime = Time.unscaledDeltaTime;

            position += _pivot.right * h * Speed * Size * deltaTime;
            position += Vector3.Cross(_pivot.right, _map.IsXY ? Vector3.forward : Vector3.up) * v * Speed * Size * deltaTime;

            position = _map.ClampPosition(position);

            _pivot.position = position;

            if (Input.GetKey(KeyCode.Q))
                _pivot.Rotate(_map.IsXY ? Vector3.back : Vector3.up, deltaTime * RotateSpeed, Space.World);
            else if (Input.GetKey(KeyCode.E))
                _pivot.Rotate(_map.IsXY ? Vector3.back : Vector3.up, -deltaTime * RotateSpeed, Space.World);

            float mouseX = Input.mousePosition.x;
            float mouseY = Input.mousePosition.y;
            float screenX = Screen.width;
            float screenY = Screen.height;
            if (mouseX < 0 || mouseX > screenX || mouseY < 0 || mouseY > screenY)
                return;

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Mathf.Abs(scroll) > 0)
                {
                    var rotation = _pivot.localRotation.eulerAngles;
                    rotation = new Vector3(Mathf.Clamp(rotation.x + deltaTime * -scroll * 100f * PitchSpeed, 5f, 89.9f), rotation.y, rotation.z);
                    _pivot.localRotation = Quaternion.Euler(rotation);
                }
            }
            else
            {
                float size = Size;
                if (scroll < 0)
                    size += deltaTime * ZoomSpeed * _camera.orthographicSize;
                else if (scroll > 0)
                    size -= deltaTime * ZoomSpeed * _camera.orthographicSize;
                Size = Mathf.Clamp(size, MinZoom, MaxZoom);
            }
        }

        private void LateUpdate()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(2))
            {
                _previousMousePosition = GetMousePosition();
            }
            else if (Input.GetMouseButton(2))
            {
                var position = GetMousePosition();
                var delta = _previousMousePosition - position;

                _pivot.Translate(delta, Space.World);
                _previousMousePosition = position + delta;

                _pivot.position = _map.ClampPosition(_pivot.position);
            }
        }

        public Ray GetRay() => _camera.ScreenPointToRay(Input.mousePosition);
        public Vector3 GetMousePosition()
        {
            var plane = new Plane(_map.IsXY ? Vector3.forward : Vector3.up, Vector3.zero);
            var ray = GetRay();

            plane.Raycast(ray, out float distance);
            return ray.GetPoint(distance);
        }
        public Vector2 GetMouseScreenPosition() => Input.mousePosition;
        public Vector2Int GetMouseGridPosition() => Dependencies.Get<IGridPositions>().GetGridPosition(GetMousePosition());

        public void SetCulling(LayerMask layerMask) => _camera.cullingMask = layerMask;
        public void ResetCulling() => _camera.cullingMask = _defaultCulling;
    }
}