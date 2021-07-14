using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// default calculator for scores
    /// starts calculations in a checker and spreads them out over multiple frames
    /// </summary>
    public class DefaultScoresCalculator : MonoBehaviour, IScoresCalculator
    {
        public event Action Calculated;

        private Dictionary<Score, int> _values = new Dictionary<Score, int>();

        protected virtual void Awake()
        {
            Dependencies.Register<IScoresCalculator>(this);
        }

        protected virtual void Start()
        {
            foreach (var item in Dependencies.Get<IObjectSet<Score>>().Objects)
            {
                _values.Add(item, item.Calculate());
            }

            StartCoroutine(calculate());
        }

        public int GetValue(Score score) => _values.ContainsKey(score) ? _values[score] : 0;

        private IEnumerator calculate()
        {
            var settings = Dependencies.GetOptional<IGameSettings>();

            while (true)
            {
                yield return new WaitForSecondsRealtime(settings?.CheckInterval ?? 1f);

                foreach (var score in _values.Keys.ToList())
                {
                    _values[score] = score.Calculate();
                    yield return null;
                }

                Calculated?.Invoke();
            }
        }
    }
}