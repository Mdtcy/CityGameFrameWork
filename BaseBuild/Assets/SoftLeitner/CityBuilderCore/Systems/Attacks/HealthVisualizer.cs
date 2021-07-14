using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// moves the transform over the target and visualizes its health in bar and/or text form
    /// </summary>
    public class HealthVisualizer : MonoBehaviour
    {
        [Tooltip("displays the health in text form, optional")]
        public TMPro.TMP_Text HealthText;
        [Tooltip("displays the health in bar form, make sure it is full in the prefab, optional")]
        public RectTransform BarTransform;

        public IHealther Healther { get; private set; }

        private Vector2 _sizeFull;
        private IMainCamera _mainCamera;

        public void InitializeHealth(IHealther healther)
        {
            Healther = healther;
            updateVisuals();
        }

        private void Start()
        {
            _mainCamera = Dependencies.Get<IMainCamera>();

            if (BarTransform)
                _sizeFull = BarTransform.sizeDelta;

            updateVisuals();
        }
        private void LateUpdate()
        {
            updateVisuals();
        }

        private void updateVisuals()
        {
            if (Healther == null || _mainCamera == null)
                return;

            transform.position = RectTransformUtility.WorldToScreenPoint(_mainCamera.Camera, Healther.HealthPosition);

            if (HealthText)
                HealthText.text = Healther.CurrentHealth.ToString();

            if (BarTransform)
                BarTransform.sizeDelta = Vector2.Lerp(Vector2.zero, _sizeFull, Healther.CurrentHealth / Healther.TotalHealth);
        }
    }
}