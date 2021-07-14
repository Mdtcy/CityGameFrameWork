using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    public static class Extensions
    {
        public static int GetActiveChildCount(this Transform transform)
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    count++;
            }
            return count;
        }
        public static bool HasActiveChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    return true;
            }
            return false;
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static int GetItemQuantity(this IEnumerable<ItemQuantity> items, Item item)
        {
            var entry = items.FirstOrDefault(i => i.Item == item);
            if (entry == null)
                return 0;
            else
                return entry.Quantity;
        }

        public static float GetUnitAmount(this IEnumerable<ItemQuantity> items, Item item)
        {
            var entry = items.FirstOrDefault(i => i.Item == item);
            if (entry == null)
                return 0;
            else
                return entry.UnitQuantity;
        }

        public static void AddQuantity(this IList<ItemQuantity> items, Item item, int quantity)
        {
            var entry = items.FirstOrDefault(i => i.Item == item);
            if (entry == null)
                items.Add(new ItemQuantity(item, quantity));
            else
                entry.Quantity += quantity;
        }

        public static void RemoveQuantity(this IList<ItemQuantity> items, Item item, int quantity)
        {
            var entry = items.FirstOrDefault(i => i.Item == item);
            if (entry == null)
                return;
            entry.Quantity -= quantity;
            if (entry.Quantity <= 0)
                items.Remove(entry);
        }

        public static bool ContainsQuantity(this IEnumerable<ItemQuantity> storage, Item item, int quantity)
        {
            foreach (var storedItem in storage)
            {
                if (storedItem == null || storedItem.Item != item)
                    continue;

                quantity -= storedItem.Quantity;
                if (quantity <= 0)
                    return true;
            }

            return false;
        }

        public static bool SubtractQuantity(this IEnumerable<ItemStack> stacks, Item item, int quantity)
        {
            if (!stacks.Select(s => s.Items).ContainsQuantity(item, quantity))
                return false;

            foreach (var stack in stacks.OrderBy(s => s.FillDegree))
            {
                quantity = stack.SubtractQuantity(item, quantity);
                if (quantity == 0)
                    break;
            }

            return true;
        }

        public static int SubtractAvailableQuantity(this IEnumerable<ItemStack> stacks, Item item, int quantity)
        {
            int wantedAmount = quantity;
            foreach (var stack in stacks.OrderBy(s => s.FillDegree))
            {
                quantity = stack.SubtractQuantity(item, quantity);
                if (quantity == 0)
                    break;
            }

            return wantedAmount - quantity;
        }

        public static bool FitsQuantity(this IEnumerable<ItemStack> stacks, Item item, int amount)
        {
            foreach (var stack in stacks)
            {
                if (stack.HasItems && stack.Items.Item != item)
                    continue;//theres a different item in that stack

                int capacity = stack.UnitCapacity * item.UnitSize;

                if (stack.Items != null)
                    capacity -= stack.Items.Quantity;

                amount -= capacity;

                if (amount <= 0)
                    return true;
            }

            return false;
        }

        public static int AddQuantity(this IEnumerable<ItemStack> stacks, Item item, int amount)
        {
            if (!stacks.FitsQuantity(item, amount))
                return amount;

            foreach (var stack in stacks)
            {
                if (stack.HasItems && stack.Items.Item != item)
                    continue;//theres a different item in that stack

                amount = stack.AddQuantity(item, amount);
                if (amount == 0)
                    break;
            }

            return amount;
        }

        public static int GetMaxAxisDistance(this Vector2Int position, Vector2Int other)
        {
            return Mathf.Max(Mathf.Abs(other.x - position.x), Mathf.Abs(other.y - position.y));
        }

        public static Coroutine StartChecker(this MonoBehaviour behaviour, Action onCheck, float? interval = null)
        {
            return behaviour.StartCoroutine(check(onCheck, interval ?? Dependencies.Get<IGameSettings>().CheckInterval));
        }
        private static IEnumerator check(Action onCheck, float interval)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, interval));

            while (true)
            {
                yield return new WaitForSeconds(interval);
                onCheck();
            }
        }
        public static Coroutine StartChecker(this MonoBehaviour behaviour, Func<bool> onCheck, float? interval = null)
        {
            return behaviour.StartCoroutine(check(onCheck, interval ?? Dependencies.Get<IGameSettings>().CheckInterval));
        }
        private static IEnumerator check(Func<bool> onCheck, float interval)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, interval));

            while (true)
            {
                yield return new WaitForSeconds(interval);
                if (!onCheck())
                    break;
            }
        }

        public static void Delay(this MonoBehaviour behaviour, float seconds, Action action)
        {
            behaviour.StartCoroutine(delay(seconds, action));
        }
        private static IEnumerator delay(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action();
        }

        public static void Delay(this MonoBehaviour behaviour, int frames, Action action)
        {
            behaviour.StartCoroutine(delay(frames, action));
        }
        private static IEnumerator delay(int frames, Action action)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
            action();
        }

        public static int GetQuantity(this IEnumerable<PopulationHousing> housings, Population population, bool includeReserved = false)
        {
            var housing = housings.FirstOrDefault(h => h.Population == population);
            if (housing == null)
                return 0;

            if (includeReserved)
                return housing.Quantity + housing.Reserved;
            else
                return housing.Quantity;
        }
        public static int GetRemainingCapacity(this IEnumerable<PopulationHousing> housings, Population population)
        {
            var housing = housings.FirstOrDefault(h => h.Population == population);
            if (housing == null)
                return 0;

            return housing.GetRemainingCapacity();
        }

        /// <summary>
        /// reserves up to quantity and returns the remainder
        /// </summary>
        /// <param name="housings"></param>
        /// <param name="population"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static int Reserve(this IEnumerable<PopulationHousing> housings, Population population, int quantity)
        {
            var housing = housings.FirstOrDefault(h => h.Population == population);
            if (housing == null)
                return quantity;

            int quantityReserved = Mathf.Min(housing.GetRemainingCapacity(), quantity);
            housing.Reserved += quantityReserved;
            return quantity - quantityReserved;
        }
        /// <summary>
        /// inhabits up to quantity and returns the remainder
        /// </summary>
        /// <param name="housings"></param>
        /// <param name="population"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static int Inhabit(this IEnumerable<PopulationHousing> housings, Population population, int quantity)
        {
            var housing = housings.FirstOrDefault(h => h.Population == population);
            if (housing == null)
                return quantity;

            int quantityInhabited = Mathf.Min(housing.Capacity - housing.Quantity, quantity);

            housing.Reserved = Mathf.Max(0, housing.Reserved - quantityInhabited);
            housing.Quantity += quantityInhabited;

            return quantity - quantityInhabited;
        }
        /// <summary>
        /// abandons up to quantity and returns the remainder
        /// </summary>
        /// <param name="housings"></param>
        /// <param name="population"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static int Abandon(this IEnumerable<PopulationHousing> housings, Population population, int quantity)
        {
            var housing = housings.FirstOrDefault(h => h.Population == population);
            if (housing == null)
                return 0;

            int quantityAbandoned = Mathf.Min(housing.Quantity, quantity);

            housing.Quantity -= quantityAbandoned;

            return quantity - quantityAbandoned;
        }
        /// <summary>
        /// removes an amount of population relative to the total capacity
        /// </summary>
        /// <param name="housings"></param>
        /// <param name="ratio"></param>
        public static void Kill(this IEnumerable<PopulationHousing> housings, float ratio)
        {
            foreach (var housing in housings)
            {
                housing.Quantity = Mathf.Max(0, housing.Quantity - (int)(housing.Capacity * ratio));
            }
        }

        public static void FollowPath(this MonoBehaviour behaviour, Transform pivot, IEnumerable<Vector3> path, float speed, Action finished)
        {
            behaviour.StartCoroutine(followPath(behaviour.transform, pivot, path, speed, finished));
        }
        private static IEnumerator followPath(Transform transform, Transform pivot, IEnumerable<Vector3> path, float speed, Action finished)
        {
            var positions = path.ToList();
            int index = 0;

            Vector3 last = positions[index];
            Vector3 next = positions[index + 1];

            float distance = Vector3.Distance(last, next);
            float moved = 0f;
            float step = speed * Time.deltaTime;

            transform.position = last;
            Dependencies.Get<IGridRotations>().SetRotation(pivot, next - last);

            yield return null;

            while (true)
            {
                moved += step;

                if (moved > distance)
                {
                    moved -= distance;

                    index++;

                    if (index >= positions.Count - 1)
                    {
                        transform.position = next;
                        finished();
                        yield break;
                    }
                    else
                    {
                        last = positions[index];
                        next = positions[index + 1];

                        distance = Vector3.Distance(last, next);

                        Dependencies.Get<IGridRotations>().SetRotation(pivot, next - last);
                    }
                }

                transform.position = Vector3.Lerp(last, next, moved / distance);

                yield return null;
            }
        }

        public static Vector2Int RotateBuildingPoint(this IBuilding building, Vector2Int point)
        {
            return building.Rotation.RotateBuildingPoint(building.Point, point, building.RawSize);
        }
    }
}