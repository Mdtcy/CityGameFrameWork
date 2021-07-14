using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// default implementation for <see cref="IAttackManager"/><br/>
    /// attacker targets are chosen purely on distance
    /// </summary>
    public class DefaultAttackManager : MonoBehaviour, IAttackManager
    {
        public HealthVisualizer HealthVisualizerPrefab;
        public Transform HealthVisualizerRoot;
        public bool IgnoreBlocked;

        private List<IAttacker> _attackers = new List<IAttacker>();
        private List<HealthVisualizer> _healthVisualizers = new List<HealthVisualizer>();

        protected virtual void Awake()
        {
            Dependencies.Register<IAttackManager>(this);
        }

        public void AddAttacker(IAttacker attacker)
        {
            _attackers.Add(attacker);
        }
        public void RemoveAttacker(IAttacker attacker)
        {
            _attackers.Remove(attacker);
        }

        public void AddHealther(IHealther healther)
        {
            if (HealthVisualizerPrefab)
            {
                var visualizer = Instantiate(HealthVisualizerPrefab, HealthVisualizerRoot);
                visualizer.InitializeHealth(healther);
                _healthVisualizers.Add(visualizer);
            }
        }
        public void RemoveHealther(IHealther healther)
        {
            var visualizer = _healthVisualizers.Where(h => h.Healther == healther).FirstOrDefault();
            if (visualizer == null)
                return;

            _healthVisualizers.Remove(visualizer);
            Destroy(visualizer.gameObject);
        }

        public IAttacker GetAttacker(Vector3 position, float maxDistance)
        {
            float minDistance = float.MaxValue;
            IAttacker minAttacker = null;

            foreach (var attacker in _attackers)
            {
                var distance = Vector3.Distance(attacker.Position, position);
                if (distance < minDistance && distance < maxDistance)
                {
                    minDistance = distance;
                    minAttacker = attacker;
                }
            }

            return minAttacker;
        }
        public BuildingComponentPath<IAttackable> GetAttackerPath(Vector2Int point, PathType pathType, object tag = null)
        {
            var attackables = Dependencies.Get<IBuildingManager>().GetBuildingTraitReferences<IAttackable>().OrderBy(t => Vector3.Distance(t.Instance.Building.WorldCenter, transform.position)).ToList();
            if (attackables.Count == 0)
                return null;//game must have finished

            foreach (var attackable in attackables)
            {
                var path = PathHelper.FindPath(point, attackable.Instance.Building, pathType, tag);
                if (path != null)
                    return new BuildingComponentPath<IAttackable>(attackable, path);
            }

            if (IgnoreBlocked)
                return null;
            else
                return new BuildingComponentPath<IAttackable>(attackables[0], PathHelper.FindPath(point, attackables[0].Instance.Building, PathType.None));
        }
    }
}