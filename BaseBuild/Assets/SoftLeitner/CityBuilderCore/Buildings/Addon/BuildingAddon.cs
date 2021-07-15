using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// 运行时添加，替换之后延续，可用于特效，状态，动画
    /// temporary building parts that are added and removed at runtime and carry over when a building is replaced<br/>
    /// can be used for effects, statuses, animations, ...
    /// </summary>
    public abstract class BuildingAddon : KeyedBehaviour, ISaveData
    {
        public bool Center;
        public bool Scale;

        public IBuilding Building { get; set; }

        public void Remove()
        {
            Building.RemoveAddon(this);
        }

        public virtual void InitializeAddon()
        {
            if (Center)
                transform.position = Building.WorldCenter;
            if (Scale)
                transform.localScale = Dependencies.Get<IMap>().IsXY ? new Vector3(Building.Size.x, Building.Size.y, 1) : new Vector3(Building.Size.x, 1, Building.Size.y);
        }
        public virtual void TerminateAddon()
        {
            Destroy(gameObject);
        }
        public virtual void OnReplacing(Transform parent, IBuilding replacement)
        {
            transform.SetParent(parent);
            Building = replacement;
        }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }

        #region Saving
        public virtual string SaveData() => string.Empty;
        public virtual void LoadData(string json) { }
        #endregion
    }
}