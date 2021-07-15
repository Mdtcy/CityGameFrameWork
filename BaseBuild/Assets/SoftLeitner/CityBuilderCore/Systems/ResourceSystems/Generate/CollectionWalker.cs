﻿using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// 四处游荡，从<see cref="IGenerationComponent"/><br/>
    /// 收集的Item种类必须配置<see cref="Items"/>
    /// </summary>
    public class CollectionWalker : BuildingComponentWalker<IGenerationComponent>
    {
        public ItemStorage Storage;
        public Item[] Items;

        public override ItemStorage ItemStorage => Storage;

        protected override void onComponentEntered(IGenerationComponent buildingComponent)
        {
            base.onComponentEntered(buildingComponent);

            buildingComponent.Collect(Storage, Items);
        }

        public override string GetDebugText() => Storage.GetDebugText();

        #region Saving
        [Serializable]
        public class CollectionWalkerData : RoamingWalkerData
        {
            public ItemStorage.ItemStorageData Storage;
            public string[] Items;
        }

        public override string SaveData()
        {
            return JsonUtility.ToJson(new CollectionWalkerData()
            {
                WalkerData = savewalkerData(),
                State = (int)_state,
                Storage = Storage.SaveData(),
                Items = Items.Select(w => w.Key).ToArray()
            });
        }
        public override void LoadData(string json)
        {
            base.LoadData(json);

            var data = JsonUtility.FromJson<CollectionWalkerData>(json);
            var items = Dependencies.Get<IKeyedSet<Item>>();

            Storage.LoadData(data.Storage);

            Items = data.Items.Select(k => items.GetObject(k)).ToArray();
        }
        #endregion
    }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class ManualCollectionWalkerSpawner : ManualWalkerSpawner<CollectionWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class CyclicCollectionWalkerSpawner : CyclicWalkerSpawner<CollectionWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class PooledCollectionWalkerSpawner : PooledWalkerSpawner<CollectionWalker> { }
}