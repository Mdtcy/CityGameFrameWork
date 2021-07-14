using System;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public class LayerValues
    {
        private Dictionary<Vector2Int, LayerPosition> _affectedPositions;
        private Dictionary<ILayerAffector, Dictionary<Vector2Int, int>> _affectorValues;

        private int[,] _baseValues;

        public LayerValues(Vector2Int size)
        {
            _affectedPositions = new Dictionary<Vector2Int, LayerPosition>();
            _affectorValues = new Dictionary<ILayerAffector, Dictionary<Vector2Int, int>>();
            _baseValues = new int[size.x, size.y];
        }

        public void AddTiles(LayerAffectorTile affectingTile)
        {
            for (int x = 0; x < _baseValues.GetLength(0); x++)
            {
                for (int y = 0; y < _baseValues.GetLength(1); y++)
                {
                    var value = affectingTile.GetValue(new Vector2Int(x, y));
                    if (Math.Abs(value) == 0)
                        continue;

                    var currentValue = _baseValues[x, y];
                    if (affectingTile.Layer.IsCumulative)
                        _baseValues[x, y] = currentValue + value;
                    else if (Math.Abs(currentValue) < Math.Abs(value))
                        _baseValues[x, y] = value;
                }
            }
        }

        public IEnumerable<Tuple<Vector2Int, int>> GetValues(Layer layer)
        {
            for (int x = 0; x < _baseValues.GetLength(0); x++)
            {
                for (int y = 0; y < _baseValues.GetLength(1); y++)
                {
                    var position = new Vector2Int(x, y);
                    var value = GetValue(position, layer);
                    if (value != 0)
                        yield return Tuple.Create(position, value);
                }
            }
        }
        public int GetValue(Vector2Int position, Layer layer)
        {
            if (position.x < 0 || position.y < 0)
                return 0;
            if (position.x >= _baseValues.GetLength(0) || position.y >= _baseValues.GetLength(1))
                return 0;

            var baseValue = _baseValues[position.x, position.y];

            if (!_affectedPositions.ContainsKey(position))
                return baseValue;

            var affectedValue = _affectedPositions[position].Value;

            if (layer.IsCumulative)
                return baseValue + affectedValue;
            else if (Math.Abs(baseValue) > Math.Abs(affectedValue))
                return baseValue;
            else
                return affectedValue;
        }
        public int GetAffectorValue(Vector2Int position, ILayerAffector affector)
        {
            if (!_affectorValues.ContainsKey(affector))
                return 0;
            if (!_affectorValues[affector].ContainsKey(position))
                return 0;
            return _affectorValues[affector][position];
        }
        public LayerKey GetKey(Vector2Int position)
        {
            LayerKey key = null;

            var baseValue = _baseValues[position.x, position.y];
            if (baseValue != 0)
                key = new LayerKey(baseValue);

            var positions = _affectedPositions;
            if (!positions.ContainsKey(position))
                return key;

            if (key == null)
                key = new LayerKey(0);

            foreach (var affector in positions[position].Affectors)
            {
                key.AddAffector(affector, GetAffectorValue(position, affector));
            }

            return key;
        }

        public void Register(ILayerAffector affector)
        {
            Dictionary<ILayerDependency, List<Vector2Int>> dependencies = new Dictionary<ILayerDependency, List<Vector2Int>>();

            var layerManager = Dependencies.Get<ILayerManager>();
            var values = affector.GetValues();

            _affectorValues.Add(affector, values);

            foreach (var positionValue in values)
            {
                var position = positionValue.Key;
                var value = positionValue.Value;
                if (!_affectedPositions.ContainsKey(position))
                    _affectedPositions.Add(position, new LayerPosition(this));
                _affectedPositions[position].AddAffector(position, value, affector);

                foreach (var dependency in layerManager.GetDependencies(position))
                {
                    if (!dependencies.ContainsKey(dependency))
                        dependencies.Add(dependency, new List<Vector2Int>());
                    dependencies[dependency].Add(position);
                }
            }

            if (Dependencies.GetOptional<IGameSaver>()?.IsLoading != true)
            {
                foreach (var dependency in dependencies)
                {
                    dependency.Key.CheckLayers(dependency.Value);
                }
            }
        }
        public void Deregister(ILayerAffector affector)
        {
            Dictionary<ILayerDependency, List<Vector2Int>> dependencies = new Dictionary<ILayerDependency, List<Vector2Int>>();

            var layerManager = Dependencies.Get<ILayerManager>();

            if (!_affectorValues.ContainsKey(affector))
                return;

            foreach (var positionValue in _affectorValues[affector])
            {
                var position = positionValue.Key;
                var value = positionValue.Value;
                if (!_affectedPositions.ContainsKey(position))
                    continue;
                var layerPosition = _affectedPositions[position];
                layerPosition.RemoveAffector(position, value, affector);
                if (layerPosition.Affectors.Count == 0)
                    _affectedPositions.Remove(position);

                foreach (var dependency in layerManager.GetDependencies(position))
                {
                    if (!dependencies.ContainsKey(dependency))
                        dependencies.Add(dependency, new List<Vector2Int>());
                    dependencies[dependency].Add(position);
                }
            }

            _affectorValues.Remove(affector);

            if (!Dependencies.Get<IGameSaver>().IsLoading)
            {
                foreach (var dependency in dependencies)
                {
                    dependency.Key.CheckLayers(dependency.Value);
                }
            }
        }
    }
}