using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    public class ExpandableBuilding : Building
    {
        public ExpandableBuildingInfo ExpandableInfo => (ExpandableBuildingInfo)Info;

        public override Vector2Int RawSize => Info.Size + Expansion + ExpandableInfo.SizePost;

        private Vector2Int _expansion;
        public Vector2Int Expansion
        {
            get { return _expansion; }
            set
            {
                _expansion = value;
                adjustPivot();
                ExpansionChanged?.Invoke(value);
            }
        }

        public event Action<Vector2Int> ExpansionChanged;

        public override void Initialize()
        {
            base.Initialize();

            adjustPivot();
        }
        protected override void onReplacing(IBuilding replacement)
        {
            base.onReplacing(replacement);

            if (replacement is ExpandableBuilding expandable)
                expandable.Expansion = Expansion;
        }

        public class ExpandableBuildingData : BuildingData
        {
            public Vector2Int Expansion;
        }

        private void adjustPivot()
        {
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new ExpandableBuildingData()
            {
                Expansion = Expansion,
                Components = _components.Select(c =>
                {
                    var data = c.SaveData();
                    if (string.IsNullOrWhiteSpace(data))
                        return null;

                    return new BuildingComponentMetaData()
                    {
                        Key = c.Key,
                        Data = data
                    };
                }).Where(d => d != null).ToArray(),
                Addons = _addons.Select(a =>
                {
                    return new BuildingAddonMetaData()
                    {
                        Key = a.Key,
                        Data = a.SaveData()
                    };
                }).Where(d => d != null).ToArray()
            });
        }

        public override void LoadData(string json)
        {
            base.LoadData(json);
        }
    }
}
