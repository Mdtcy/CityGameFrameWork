using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// 物品的通用容器
    /// 定义传输Item的各种方法，以抽象不同的存储模式
    /// </summary>
    [Serializable]
    public class ItemStorage
    {
        private ItemStorage _globalStorage => Dependencies.Get<IGlobalStorage>().Items;

        public bool IsStackedStorage => Mode == ItemStorageMode.Stacked;
        public bool IsFreeStorage => Mode == ItemStorageMode.Free || Mode == ItemStorageMode.FreeItemCapped || Mode == ItemStorageMode.FreeUnitCapped;
        public bool IsGlobalStorage => Mode == ItemStorageMode.Global;

        public ItemStorageMode Mode = ItemStorageMode.FreeUnitCapped;
        [Tooltip("the number of stacks in stacked mode, otherwise irrelevant")]
        public int StackCount;
        [Tooltip("number of units in unitCapped, of quantity in itemCapped and units/stack in stacked mode")]
        public int Capacity = 1;

        public ItemStack[] Stacks
        {
            get
            {
                if (_stacks == null)
                    setupStacks();
                return _stacks;
            }
        }

        private ItemStack[] _stacks = null;
        private List<ItemQuantity> _freeItems = new List<ItemQuantity>();
        private List<ItemQuantity> _reservedCapacity = new List<ItemQuantity>();
        private List<ItemQuantity> _reservedQuantity = new List<ItemQuantity>();

        public bool HasItem(Item item)
        {
            return GetItem().Any(i => i == item);
        }
        public bool HasItems(Item item, int quantity)
        {
            return GetItemQuantity(item) - _reservedQuantity.GetItemQuantity(item) >= quantity;
        }
        public bool HasItems()
        {
            return GetItem().Any();
        }

        public int GetItemQuantity(Item item)
        {
            var items = GetItems().FirstOrDefault(i => i.Item == item);
            if (items == null)
                return 0;
            else
                return items.Quantity;
        }
        public float GetUnitQuantity(Item item)
        {
            return (float)GetItemQuantity(item) / item.UnitSize;
        }

        public int GetItemCapacity(Item item, float ratio = 1f)
        {
            switch (Mode)
            {
                case ItemStorageMode.Stacked:
                    var stackCount = Mathf.RoundToInt(ratio * StackCount);
                    return stackCount * Capacity * item.UnitSize;
                case ItemStorageMode.Free:
                    return int.MaxValue;
                case ItemStorageMode.FreeItemCapped:
                    return Mathf.RoundToInt(ratio * Capacity);
                case ItemStorageMode.FreeUnitCapped:
                    return Mathf.RoundToInt(ratio * Capacity) * item.UnitSize;
                case ItemStorageMode.Global:
                    return _globalStorage.GetItemCapacity(item, ratio);
                default:
                    return 0;
            }
        }

        public int GetItemCapacityOverflowing(Item item, float ratio = 1f)
        {
            switch (Mode)
            {
                case ItemStorageMode.Stacked:
                    var stackCount = Mathf.RoundToInt(ratio * StackCount);
                    var maximum = stackCount * Capacity * item.UnitSize;
                    var current = GetItemQuantity(item);

                    return current - maximum;
                case ItemStorageMode.Free:
                    if (ratio == 0f)
                        return GetItemQuantity(item);
                    else
                        return 0;
                case ItemStorageMode.FreeItemCapped:
                    return _freeItems.GetItemQuantity(item) - Mathf.RoundToInt(ratio * Capacity);
                case ItemStorageMode.FreeUnitCapped:
                    return _freeItems.GetItemQuantity(item) - Mathf.RoundToInt(ratio * Capacity) * item.UnitSize;
                case ItemStorageMode.Global:
                    return _globalStorage.GetItemCapacityOverflowing(item, ratio);
                default:
                    return 0;
            }
        }

        public int GetItemCapacityRemaining(Item item, float ratio = 1f)
        {
            switch (Mode)
            {
                case ItemStorageMode.Stacked:
                    var stackCount = Mathf.RoundToInt(ratio * StackCount);
                    var usedStacks = Stacks.Where(s => s.Items != null && s.Items.Item == item).ToList();

                    var freeStacks = Math.Min(stackCount - usedStacks.Count, Stacks.Where(s => !s.HasItems).Count());

                    return freeStacks * Capacity * item.UnitSize + usedStacks.Sum(s => s.GetItemCapacityRemaining(item));
                case ItemStorageMode.Free:
                    return int.MaxValue;
                case ItemStorageMode.FreeItemCapped:
                    return Mathf.RoundToInt(ratio * Capacity) - _freeItems.GetItemQuantity(item) - _reservedCapacity.GetItemQuantity(item);
                case ItemStorageMode.FreeUnitCapped:
                    return Mathf.RoundToInt(ratio * Capacity) * item.UnitSize - _freeItems.GetItemQuantity(item) - _reservedCapacity.GetItemQuantity(item);
                case ItemStorageMode.Global:
                    return _globalStorage.GetItemCapacityRemaining(item, ratio);
                default:
                    return 0;
            }
        }
        public float GetUnitCapacityRemaining(Item item)
        {
            return (float)GetItemCapacityRemaining(item) / item.UnitSize;
        }

        public bool FitsItems(Item item, int quantity)
        {
            return GetItemCapacityRemaining(item) >= quantity;
        }

        public IEnumerable<Item> GetItem()
        {
            if (IsGlobalStorage)
            {
                foreach (var item in _globalStorage.GetItem())
                {
                    yield return item;
                }
            }
            else if (IsFreeStorage)
            {
                foreach (var item in _freeItems.Select(i => i.Item))
                {
                    yield return item;
                }
            }
            else
            {
                List<Item> items = new List<Item>();
                foreach (var stack in Stacks)
                {
                    if (!stack.HasItems)
                        continue;
                    if (items.Contains(stack.Items.Item))
                        continue;

                    yield return stack.Items.Item;
                    items.Add(stack.Items.Item);
                }
            }

        }
        public IEnumerable<ItemQuantity> GetItems()
        {
            if (IsGlobalStorage)
            {
                return _globalStorage.GetItems();
            }
            else if (IsFreeStorage)
            {
                return _freeItems;
            }
            else
            {
                Dictionary<Item, int> items = new Dictionary<Item, int>();
                foreach (var stack in Stacks)
                {
                    if (!stack.HasItems)
                        continue;

                    if (items.ContainsKey(stack.Items.Item))
                        items[stack.Items.Item] += stack.Items.Quantity;
                    else
                        items.Add(stack.Items.Item, stack.Items.Quantity);
                }

                return items.Select(i => new ItemQuantity(i.Key, i.Value));
            }
        }
        public ItemQuantity GetItems(Item item)
        {
            return GetItems().FirstOrDefault(i => i.Item == item);
        }

        public void MoveItemsTo(ItemStorage other)
        {
            foreach (var items in GetItems().ToList())
            {
                int removed = items.Quantity - other.AddItems(items.Item, items.Quantity);
                RemoveItems(items.Item, removed);
            }
        }
        public void MoveItemsTo(ItemStorage other, Item item, int maxQuantity = int.MaxValue)
        {
            var items = GetItems().FirstOrDefault(i => i.Item == item);
            if (items == null)
                return;

            int quantity = Math.Min(items.Quantity, maxQuantity);

            int remaining = other.AddItems(items.Item, quantity);
            RemoveItems(items.Item, quantity - remaining);
        }

        /// <summary>
        /// returns remaining quantity not added
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public int AddItems(Item item, int quantity)
        {
            if (IsGlobalStorage)
            {
                return _globalStorage.AddItems(item, quantity);
            }
            else if (IsFreeStorage)
            {
                int capacity = getFreeItemCapacity(item);

                var items = _freeItems.FirstOrDefault(i => i.Item == item);

                int stored = items?.Quantity ?? 0;
                int add = Mathf.Min(capacity - stored, quantity);

                if (items == null)
                    _freeItems.Add(new ItemQuantity(item, add));
                else
                    items.Quantity += add;

                return quantity - add;
            }
            else
            {
                foreach (var stack in Stacks)
                {
                    if (stack.HasItems && stack.Items.Item != item)
                        continue;//theres a different item in that stack

                    quantity = stack.AddQuantity(item, quantity);
                    if (quantity == 0)
                        break;
                }

                return quantity;
            }
        }
        /// <summary>
        /// returns remaining quantity not removed
        /// </summary>
        /// <param name="item"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public int RemoveItems(Item item, int quantity)
        {
            if (IsGlobalStorage)
            {
                return _globalStorage.RemoveItems(item, quantity);
            }
            else if (IsFreeStorage)
            {
                var items = _freeItems.FirstOrDefault(i => i.Item == item);
                if (items == null)
                {
                    return quantity;
                }
                else
                {
                    if (items.Quantity > quantity)
                    {
                        items.Quantity -= quantity;
                        return 0;
                    }
                    else
                    {
                        _freeItems.Remove(items);
                        return quantity - items.Quantity;
                    }
                }
            }
            else
            {
                foreach (var stack in Stacks.OrderBy(s => s.FillDegree))
                {
                    quantity = stack.SubtractQuantity(item, quantity);
                    if (quantity == 0)
                        break;
                }

                return quantity;
            }
        }

        public void ReserveCapacity(Item item, int quantity)
        {
            _reservedCapacity.AddQuantity(item, quantity);
        }
        public void UnreserveCapacity(Item item, int quantity)
        {
            _reservedCapacity.RemoveQuantity(item, quantity);
        }

        public void ReserveQuantity(Item item, int quantity)
        {
            _reservedQuantity.AddQuantity(item, quantity);
        }
        public void UnreserveQuantity(Item item, int quantity)
        {
            _reservedQuantity.RemoveQuantity(item, quantity);
        }

        public string GetDebugText()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var items in GetItems())
            {
                sb.AppendLine($"{items.Item.Key}: {items.Quantity}");
            }

            return sb.ToString();
        }
        public string GetItemNames()
        {
            return string.Join(", ", GetItem().Select(i => i.Name));
        }

        private void setupStacks()
        {
            int count = IsStackedStorage ? StackCount : 0;

            _stacks = new ItemStack[count];
            for (int i = 0; i < count; i++)
            {
                _stacks[i] = new ItemStack() { UnitCapacity = Capacity };
            }
        }

        private int getFreeItemCapacity(Item item)
        {
            switch (Mode)
            {
                case ItemStorageMode.FreeItemCapped:
                    return Capacity;
                case ItemStorageMode.FreeUnitCapped:
                    return Capacity * item.UnitSize;
                default:
                    return int.MaxValue;
            }
        }

        private static ItemQuantity.ItemQuantityData[] getData(ItemStack[] stacks)
        {
            if (stacks == null)
                return null;
            return stacks.Select(s => s.GetData()).ToArray();
        }
        private static ItemQuantity.ItemQuantityData[] getData(List<ItemQuantity> items)
        {
            if (items == null)
                return null;
            return items.Select(i => i.GetData()).ToArray();
        }
        private static void getStacks(ItemQuantity.ItemQuantityData[] datas, ItemStack[] stacks)
        {
            if (datas == null)
                return;

            for (int i = 0; i < stacks.Length; i++)
            {
                var data = datas.ElementAtOrDefault(i);
                if (data == null || string.IsNullOrWhiteSpace(data.Key))
                    continue;
                stacks[i].SetData(data);
            }
        }
        private static List<ItemQuantity> getItems(ItemQuantity.ItemQuantityData[] datas)
        {
            if (datas == null)
                return null;
            return datas.Select(d => new ItemQuantity(Dependencies.Get<IKeyedSet<Item>>().GetObject(d.Key), d.Quantity)).ToList();
        }

        #region Saving
        [Serializable]
        public class ItemStorageData
        {
            public ItemQuantity.ItemQuantityData[] Stacked;
            public ItemQuantity.ItemQuantityData[] Free;
            public ItemQuantity.ItemQuantityData[] ReservedCapacity;
            public ItemQuantity.ItemQuantityData[] ReservedQuantity;
        }

        public ItemStorageData SaveData()
        {
            return new ItemStorageData()
            {
                Stacked = getData(_stacks),
                Free = getData(_freeItems),
                ReservedCapacity = getData(_reservedCapacity),
                ReservedQuantity = getData(_reservedQuantity)
            };
        }
        public void LoadData(ItemStorageData data)
        {
            getStacks(data.Stacked, Stacks);
            _freeItems = getItems(data.Free);
            _reservedCapacity = getItems(data.ReservedCapacity);
            _reservedQuantity = getItems(data.ReservedQuantity);
        }
        #endregion
    }
}