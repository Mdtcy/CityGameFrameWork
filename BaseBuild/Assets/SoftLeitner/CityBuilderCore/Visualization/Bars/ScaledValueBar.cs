using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// visualizes a building value by scaling a transform along Y<br/>
    /// </summary>
    public class ScaledValueBar : BaseValueBar
    {
        public Transform ScaleTransform;
        public MeshRenderer BarRenderer;

        public Material BarMaterial
        {
            get
            {
                return BarRenderer.material;
            }
            set
            {
                BarRenderer.material = value;
            }
        }

        private float _fullHeight;
        private Vector3 _scale;

        private void Start()
        {
            _fullHeight = ScaleTransform.localScale.y;
            _scale = ScaleTransform.localScale;

            setBar();
        }

        private void Update()
        {
            setBar();
        }

        private void setBar()
        {
            _scale.y = GetValue() / 100f * _fullHeight;
            ScaleTransform.localScale = _scale;
        }
    }
}