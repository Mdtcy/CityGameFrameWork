using System;
using System.Collections.Generic;

namespace CityBuilderCore
{
    /// <summary>
    /// explanation of the computed layer value at a point (Base+Affectors=Total)
    /// </summary>
    public class LayerKey
    {
        public int BaseValue { get; private set; }
        public int TotalValue { get; private set; }
        public IReadOnlyCollection<Tuple<int, ILayerAffector>> Affectors => _affectors;

        private List<Tuple<int, ILayerAffector>> _affectors;

        public LayerKey(int baseValue)
        {
            BaseValue = baseValue;
            TotalValue = baseValue;
            _affectors = new List<Tuple<int, ILayerAffector>>();
        }

        public void AddAffector(ILayerAffector affector, int value)
        {
            _affectors.Add(Tuple.Create(value, affector));
            TotalValue += value;
        }
    }
}