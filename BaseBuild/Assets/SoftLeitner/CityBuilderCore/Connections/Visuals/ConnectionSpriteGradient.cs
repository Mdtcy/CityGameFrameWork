using UnityEngine;
using UnityEngine.Events;

namespace CityBuilderCore
{
    public class ConnectionSpriteGradient : MonoBehaviour
    {
        public ConnectionPasserBase ConnectionPasser;
        public SpriteRenderer SpriteRenderer;
        public Gradient Gradient;
        public int Maximum;

        private void Awake()
        {
            if (ConnectionPasser)
                ConnectionPasser.PointValueChanged.AddListener(new UnityAction<Vector2Int, int>(Apply));
        }

        public void Apply(Vector2Int point, int value)
        {
            SpriteRenderer.color = Gradient.Evaluate(value / (float)Maximum);
        }
    }
}
