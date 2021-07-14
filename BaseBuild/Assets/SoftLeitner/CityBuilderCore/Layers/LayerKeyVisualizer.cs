using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    public class LayerKeyVisualizer : MonoBehaviour
    {
        public RectTransform Root;
        public GameObject Visual;
        public LayerAffectorVisualizer AffectorPrefab;

        public GameObject BaseValueObject;
        public TMPro.TMP_Text BaseValueText;
        public TMPro.TMP_Text TotalValueText;

        private ILayerManager _layerManager;
        private IMouseInput _mouseInput;
        private IMap _map;

        private Vector2Int _activeMousePosition;
        private Layer _activeLayer;
        private List<LayerAffectorVisualizer> _affectorVisualizers = new List<LayerAffectorVisualizer>();

        private void Start()
        {
            _layerManager = Dependencies.Get<ILayerManager>();
            _mouseInput = Dependencies.Get<IMouseInput>();
            _map = Dependencies.Get<IMap>();

            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!_activeLayer)
                return;

            Root.anchoredPosition = _mouseInput.GetMouseScreenPosition();

            var currentMousePositon = _mouseInput.GetMouseGridPosition();
            if (currentMousePositon == _activeMousePosition)
                return;
            _activeMousePosition = currentMousePositon;

            _affectorVisualizers.ForEach(a => Destroy(a.gameObject));
            _affectorVisualizers.Clear();

            if (!_map.IsInside(_activeMousePosition))
            {
                Visual.SetActive(false);
                return;
            }

            var key = _layerManager.GetKey(_activeLayer, _activeMousePosition);

            if (key == null)
            {
                Visual.SetActive(false);
                return;
            }

            Visual.SetActive(true);
            BaseValueObject.SetActive(key.BaseValue != 0);
            BaseValueText.text = key.BaseValue.ToString();
            TotalValueText.text = key.TotalValue.ToString();

            for (int i = 0; i < key.Affectors.Count; i++)
            {
                var affector = key.Affectors.ElementAt(i);
                var affectorVisualizer = Instantiate(AffectorPrefab, BaseValueObject.transform.parent);
                affectorVisualizer.ValueText.text = affector.Item1.ToString();
                affectorVisualizer.NameText.text = affector.Item2.Name;
                affectorVisualizer.transform.SetSiblingIndex(i + 1);
                affectorVisualizer.gameObject.SetActive(true);
                _affectorVisualizers.Add(affectorVisualizer);
            }
        }

        public void Activate(Layer layer)
        {
            _activeLayer = layer;
            _activeMousePosition = new Vector2Int(int.MaxValue, int.MaxValue);
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            _activeLayer = null;
            _activeMousePosition = new Vector2Int(int.MaxValue, int.MaxValue);
            gameObject.SetActive(false);
        }
    }
}
