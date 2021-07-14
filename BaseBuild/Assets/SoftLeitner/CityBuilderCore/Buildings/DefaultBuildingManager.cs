using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    public class DefaultBuildingManager : MonoBehaviour, IBuildingManager, IBarManager
    {
        public BuildingAddon AddingAddon;

        public float EvolutionDelay;
        public BuildingAddon EvolutionDelayAddon;
        public float DevolutionDelay;
        public BuildingAddon DevolutionDelayAddon;

        public event Action<IBuilding> Added;
        public event Action<IBuilding> Registered;
        public event Action<IBuilding> Deregistered;

        private readonly List<BuildingReference> _buildings = new List<BuildingReference>();
        private readonly Dictionary<Type, List<object>> _traits = new Dictionary<Type, List<object>>();

        private ViewBarBase _currentViewBar;
        private List<BaseValueBar> _currentBars;

        protected virtual void Awake()
        {
            Dependencies.Register<IBuildingManager>(this);
            Dependencies.Register<IBarManager>(this);
        }

        protected virtual void Update()
        {
            if (_currentViewBar)
            {
                _currentBars.ForEach(b => b.gameObject.SetActive(b.HasValue()));
            }
        }

        public T Add<T>(Vector3 position, Quaternion rotation, T prefab, Action<T> initializer = null) where T : Building
        {
            var building = Instantiate(prefab, position, rotation, transform);

            initializer?.Invoke(building);
            building.Initialize();

            Dependencies.Get<IStructureManager>().Remove(building.GetPoints(), prefab.Info.Level.Value, true);

            if (AddingAddon)
                building.AddAddon(AddingAddon);

            Added?.Invoke(building);

            return building;
        }

        public int Count(BuildingInfo info) => _buildings.Where(b => b.Instance.Info == info).Count();
        public IEnumerable<IBuilding> GetBuildings() => _buildings.Select(b => b.Instance);
        public IEnumerable<IBuilding> GetBuildings(BuildingCategory category) => _buildings.Select(b => b.Instance).Where(b => category.Buildings.Contains(b.Info));
        public IEnumerable<IBuilding> GetBuildings(BuildingInfo info) => _buildings.Where(b => b.Instance.Info == info).Select(b => b.Instance);

        public void RegisterBuilding(IBuilding building)
        {
            _buildings.Add(building.BuildingReference);

            if (_currentViewBar != null)
            {
                _currentBars.Add(instantiateBar(building.BuildingReference, _currentViewBar));
            }

            Registered?.Invoke(building);
        }
        public void DeregisterBuilding(IBuilding building)
        {
            _buildings.Remove(building.BuildingReference);

            if (_currentViewBar != null)
            {
                var bar = _currentBars.FirstOrDefault(b => b.Building == building);
                if (bar != null)
                    _currentBars.Remove(bar);
            }

            Deregistered?.Invoke(building);
        }

        public IEnumerable<IBuilding> GetBuilding(Vector2Int position)
        {
            return Dependencies.Get<IStructureManager>().GetStructures(position, 0).OfType<IBuilding>();
        }
        public BuildingReference GetBuildingReference(Guid id) => _buildings.FirstOrDefault(b => b.Instance.Id == id);

        public BuildingComponentReference<T> RegisterBuildingTrait<T>(T component)
            where T : IBuildingTrait<T>
        {
            var type = typeof(T);
            if (!_traits.ContainsKey(type))
                _traits.Add(type, new List<object>());
            var reference = new BuildingComponentReference<T>(component);
            _traits[type].Add(reference);
            return reference;
        }
        public void ReplaceBuildingTrait<T>(T component, T replacement)
            where T : IBuildingTrait<T>
        {
            if (replacement == null)
            {
                component.Reference.Instance = default;
                DeregisterBuildingTrait(component);
            }
            else
            {
                component.Reference.Instance = replacement;
                replacement.Reference = component.Reference;
            }
        }
        public void DeregisterBuildingTrait<T>(T component)
            where T : IBuildingTrait<T>
        {
            var type = typeof(T);
            if (!_traits.ContainsKey(type))
                return;
            _traits[type].Remove(component.Reference);
        }
        public IEnumerable<BuildingComponentReference<T>> GetBuildingTraitReferences<T>()
            where T : IBuildingTrait<T>
        {
            var type = typeof(T);
            if (!_traits.ContainsKey(type))
                return Enumerable.Empty<BuildingComponentReference<T>>();
            return _traits[type].Cast<BuildingComponentReference<T>>();
        }
        public IEnumerable<T> GetBuildingTraits<T>()
            where T : IBuildingTrait<T>
        {
            var type = typeof(T);
            if (!_traits.ContainsKey(type))
                return Enumerable.Empty<T>();
            return _traits[type].Cast<BuildingComponentReference<T>>().Select(r => r.Instance);
        }

        public void ActivateBars(ViewBarBase viewBar)
        {
            ClearBars();
            _currentViewBar = viewBar;
            _currentBars = new List<BaseValueBar>();
            foreach (var buildingReference in _buildings)
            {
                _currentBars.Add(instantiateBar(buildingReference, _currentViewBar));
            }
        }
        public void ClearBars()
        {
            _currentViewBar = null;
            _currentBars?.ForEach(b => Destroy(b.gameObject));
            _currentBars = null;
        }
        private BaseValueBar instantiateBar(BuildingReference buildingReference, ViewBarBase viewBar)
        {
            var bar = Instantiate(viewBar.Bar, buildingReference.Instance.WorldCenter, Quaternion.identity, buildingReference.Instance.Root);
            bar.Initialize(buildingReference, viewBar.BuildingValue);
            bar.gameObject.SetActive(bar.HasValue());
            return bar;
        }

        public bool HasEvolutionDelay(bool direction)
        {
            if (direction)
                return EvolutionDelay > 0f;
            else
                return DevolutionDelay > 0f;
        }
        public float GetEvolutionDelay(bool direction)
        {
            if (direction)
                return EvolutionDelay;
            else
                return DevolutionDelay;
        }
        public string AddEvolutionAddon(IBuilding building, bool direction)
        {
            BuildingAddon addon = direction ? EvolutionDelayAddon : DevolutionDelayAddon;

            if (addon)
                return building.AddAddon(addon).Key;
            else
                return null;
        }

        #region Saving
        [Serializable]
        public class BuildingsData
        {
            public BuildingMetaData[] Buildings;
        }
        [Serializable]
        public class BuildingMetaData
        {
            public string Id;
            public string Key;
            public Vector2Int Position;
            public int Rotation;
            public string Data;
        }

        public string SaveData()
        {
            return JsonUtility.ToJson(new BuildingsData()
            {
                Buildings = _buildings.Select(b =>
                {
                    BuildingMetaData metaData = new BuildingMetaData
                    {
                        Id = b.Instance.Id.ToString(),
                        Key = b.Instance.Info.Key,
                        Position = b.Instance.Point,
                        Rotation = b.Instance.Rotation.State,
                        Data = b.Instance.SaveData()
                    };

                    return metaData;
                }).ToArray()
            });
        }
        public void LoadData(string json)
        {
            var buildingsData = JsonUtility.FromJson<BuildingsData>(json);

            foreach (var building in _buildings.ToList())
            {
                building.Instance.Terminate();
            }

            var buildingDatas = new List<(IBuilding building, string data)>();
            var buildings = Dependencies.Get<IKeyedSet<BuildingInfo>>();

            foreach (var metaData in buildingsData.Buildings)
            {
                var info = buildings.GetObject(metaData.Key);
                if (info == null)
                    continue;

                var rotation = new BuildingRotation(metaData.Rotation);
                var building = Instantiate(info.Prefab, Dependencies.Get<IGridPositions>().GetWorldPosition(rotation.RotateOrigin(metaData.Position, info.Size)), rotation.GetRotation(), transform);
                building.Initialize();
                building.Id = new Guid(metaData.Id);
                buildingDatas.Add((building, metaData.Data));
            }

            foreach (var (building, data) in buildingDatas)
            {
                building.LoadData(data);
            }
        }
        #endregion
    }
}