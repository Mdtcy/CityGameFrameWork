using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// collection of points or positions that can be followed by a walker
    /// </summary>
    public class WalkingPath
    {
        public int Length => _isPointPath ? _points.Length : _positions.Length;

        public Vector2Int StartPoint => _isPointPath ? _points.FirstOrDefault() : _grid.GetGridPosition(StartPosition);
        public Vector3 StartPosition => _isPointPath ? _grid.GetWorldPosition(StartPoint) : _positions.FirstOrDefault();

        public Vector2Int EndPoint => _isPointPath ? _points.LastOrDefault() : _grid.GetGridPosition(EndPosition);
        public Vector3 EndPosition => _isPointPath ? _grid.GetWorldPosition(EndPoint) : _positions.LastOrDefault();

        private bool _isPointPath;
        private Vector2Int[] _points;
        private Vector3[] _positions;

        private IGridPositions _grid;

        public WalkingPath(Vector2Int[] points)
        {
            _grid = Dependencies.Get<IGridPositions>();

            _isPointPath = true;
            _points = points;
        }
        public WalkingPath(Vector3[] positions)
        {
            _grid = Dependencies.Get<IGridPositions>();

            _isPointPath = false;
            _positions = positions;
        }

        public Vector2Int GetPoint(int index)
        {
            if (_isPointPath)
                return _points.ElementAtOrDefault(index);
            else
                return _grid.GetGridPosition(GetPosition(index));
        }

        public Vector3 GetPosition(int index)
        {
            if (_isPointPath)
                return _grid.GetWorldPosition(GetPoint(index));
            else
                return _positions.ElementAtOrDefault(index);
        }

        public Vector3 GetPosition(int index, float time, float timePerStep) => Vector3.Lerp(GetPreviousPosition(index), GetNextPosition(index), time / timePerStep);

        public bool HasEnded(int index) => index >= Length - 1;
        public Vector2Int GetPreviousPoint(int index)
        {
            if (_isPointPath)
                return _points.ElementAtOrDefault(index);
            else
                return _grid.GetGridPosition(GetPreviousPosition(index));
        }
        public Vector2Int GetNextPoint(int index)
        {
            if (_isPointPath)
                return index + 1 < _points.Length ? _points.ElementAtOrDefault(index + 1) : _points.Last();
            else
                return _grid.GetGridPosition(GetNextPosition(index));
        }
        public Vector3 GetPreviousPosition(int index)
        {
            if (_isPointPath)
                return _grid.GetWorldPosition(GetPreviousPoint(index));
            else
                return _positions.ElementAtOrDefault(index);
        }
        public Vector3 GetNextPosition(int index)
        {
            if (_isPointPath)
                return _grid.GetWorldPosition(GetNextPoint(index));
            else
                return index + 1 < _positions.Length ? _positions.ElementAtOrDefault(index + 1) : _positions.Last();
        }
        public float GetDistance(int index) => Vector3.Distance(GetPreviousPosition(index), GetNextPosition(index));
        public Vector3 GetDirection(int index)
        {
            return GetNextPosition(index) - GetPreviousPosition(index);
        }

        public void Reverse()
        {
            _points.Reverse();
        }

        public IEnumerator Walk(Walker walker, float delay, Action finished, Action<Vector2Int> moved = null)
        {
            walker.transform.position = StartPosition;
            if (Length > 1)
                Dependencies.Get<IGridRotations>().SetRotation(walker.Pivot, GetDirection(0));

            walker.CurrentWalking = new WalkingState() { Path = this };

            if (delay > 0f)
            {
                walker.CurrentWaiting = new WaitingState() { SetTime = walker.Delay };
            }

            yield return ContinueWalk(walker, finished, moved);
        }
        public IEnumerator ContinueWalk(Walker walker, Action finished, Action<Vector2Int> moved = null)
        {
            if (walker.CurrentWaiting != null)
            {
                yield return walker.CurrentWaiting.Wait();
                walker.CurrentWaiting = null;
            }

            walker.IsWalking = true;

            var rotations = Dependencies.GetOptional<IGridRotations>();
            var heights = Dependencies.GetOptional<IGridHeights>();
            var w = walker.CurrentWalking;

            var last = GetPreviousPosition(w.Index);
            var next = GetNextPosition(w.Index);

            var distance = GetDistance(w.Index);

            while (true)
            {
                w.Moved += Time.deltaTime * walker.Speed;

                if (w.Moved >= distance)
                {
                    moved?.Invoke(GetNextPoint(w.Index));

                    w.Moved -= distance;
                    w.Index++;

                    if (HasEnded(w.Index))
                    {
                        walker.transform.position = EndPosition;
                        walker.CurrentWalking = null;
                        walker.IsWalking = false;
                        finished();
                        yield break;
                    }
                    else
                    {
                        last = GetPreviousPosition(w.Index);
                        next = GetNextPosition(w.Index);

                        distance = GetDistance(w.Index);

                        rotations?.SetRotation(walker.Pivot, GetDirection(w.Index));
                    }
                }

                var position = Vector3.Lerp(last, next, w.Moved / distance);

                walker.transform.position = position;
                heights?.SetHeight(walker.Pivot, position, walker.PathType, walker.HeightOverride);

                yield return null;
            }
        }

        public static IEnumerator TryWalk(Walker walker, float delay, Vector2Int waitPosition, Func<WalkingPath> pathGetter, Action planned, Action finished, Action canceled = null, Action<Vector2Int> moved = null)
        {
            if (walker.CurrentWaiting == null)
                walker.CurrentWaiting = new WaitingState() { SetTime = walker.MaxWait };

            do
            {
                var path = pathGetter();
                if (path != null)
                {
                    var remainingDelay = delay - walker.CurrentWaiting.WaitTime;
                    walker.CurrentWaiting = null;
                    planned?.Invoke();
                    yield return path.Walk(walker, remainingDelay, finished, moved);
                    yield break;
                }
                else
                {
                    walker.transform.position = Dependencies.Get<IGridPositions>().GetWorldPosition(waitPosition);
                }

                yield return new WaitForSeconds(1f);
                walker.CurrentWaiting.WaitTime++;
            }
            while (walker.CurrentWaiting.IsFinished);

            walker.CurrentWaiting = null;
            if (canceled == null)
                finished();
            else
                canceled();
        }
        public static IEnumerator TryWalk(Walker walker, float delay, Vector2Int position, IBuilding structure, PathType pathType, object pathTag, Action planned, Action finished, Action canceled = null, Action<Vector2Int> moved = null)
        {
            yield return TryWalk(walker, delay, position, () => PathHelper.FindPath(position, structure, pathType, pathTag), planned, finished, canceled, moved);
        }

        public static IEnumerator Roam(Walker walker, float delay, Vector2Int start, int maxMemory, int maxSteps, Action finished, Action<Vector2Int> moved = null)
        {
            walker.CurrentRoaming = new RoamingState();

            if (delay > 0f)
            {
                walker.CurrentWaiting = new WaitingState() { SetTime = walker.Delay };
            }

            var roaming = walker.CurrentRoaming;

            roaming.Steps = 0;
            roaming.Moved = 0f;

            memorize(start, roaming.Memory, maxMemory);

            roaming.Current = start;
            roaming.Next = roam(roaming.Current, roaming.Memory, walker.PathType, walker.PathTag);

            walker.transform.position = Dependencies.Get<IGridPositions>().GetWorldPosition(roaming.Current);

            yield return null;

            yield return ContinueRoam(walker, maxMemory, maxSteps, finished, moved);
        }
        public static IEnumerator ContinueRoam(Walker walker, int maxMemory, int maxSteps, Action finished, Action<Vector2Int> moved = null)
        {
            if (walker.CurrentWaiting != null)
            {
                yield return walker.CurrentWaiting.Wait();
                walker.CurrentWaiting = null;
            }

            walker.IsWalking = true;

            var positions = Dependencies.Get<IGridPositions>();
            var rotations = Dependencies.GetOptional<IGridRotations>();
            var heights = Dependencies.GetOptional<IGridHeights>();
            var roaming = walker.CurrentRoaming;

            var from = positions.GetWorldPosition(roaming.Current);
            var to = positions.GetWorldPosition(roaming.Next);
            var distance = Vector3.Distance(from, to);

            while (roaming.Steps < maxSteps)
            {
                roaming.Moved += Time.deltaTime * walker.Speed;

                if (roaming.Moved >= distance)
                {
                    moved?.Invoke(roaming.Next);

                    roaming.Moved -= distance;
                    roaming.Current = roaming.Next;

                    roaming.Steps++;

                    if (roaming.Steps >= maxSteps)
                    {
                        break;
                    }
                    else
                    {
                        memorize(roaming.Current, roaming.Memory, maxMemory);
                        roaming.Next = roam(roaming.Current, roaming.Memory, walker.PathType, walker.PathTag);
                    }

                    from = positions.GetWorldPosition(roaming.Current);
                    to = positions.GetWorldPosition(roaming.Next);
                    distance = Vector3.Distance(from, to);
                }

                Vector3 position = Vector3.Lerp(from, to, roaming.Moved / distance);

                walker.transform.position = position;
                rotations?.SetRotation(walker.Pivot, to - from);
                heights?.SetHeight(walker.Pivot, position, walker.PathType, walker.HeightOverride);

                yield return null;
            }

            walker.transform.position = positions.GetWorldPosition(roaming.Current);
            yield return null;

            walker.CurrentRoaming = null;
            walker.IsWalking = false;
            finished();
        }
        private static void memorize(Vector2Int point, List<Vector2Int> memory, int maxMemory)
        {
            memory.Remove(point);
            memory.Add(point);

            while (memory.Count > maxMemory)
                memory.RemoveAt(0);
        }

        private static Vector2Int roam(Vector2Int current, List<Vector2Int> memory, PathType pathType, object pathTag = null)
        {
            var options = PathHelper.GetAdjacent(current, pathType, pathTag).ToList();

            if (options.Count == 0)
            {
                return current;
            }

            var firstTime = options.Where(o => !memory.Contains(o)).ToList();

            if (firstTime.Count == 0)
            {
                return options.OrderBy(o => memory.IndexOf(o)).First();
            }
            else if (firstTime.Count == 1)
            {
                return firstTime[0];
            }
            else
            {
                return firstTime[UnityEngine.Random.Range(0, firstTime.Count)];
            }
        }

        #region Saving
        [Serializable]
        public class WalkingPathData
        {
            public bool IsPointPath;
            public List<Vector2Int> Points;
            public List<Vector3> Positions;
        }

        public WalkingPathData GetData()
        {
            return new WalkingPathData()
            {
                IsPointPath = _isPointPath,
                Points = _isPointPath ? _points.ToList() : null,
                Positions = _isPointPath ? null : _positions?.ToList()
            };
        }
        public static WalkingPath FromData(WalkingPathData data)
        {
            if (data == null)
                return null;

            if (data.IsPointPath)
                return new WalkingPath(data.Points.ToArray());
            else
                return new WalkingPath(data.Positions.ToArray());
        }
        #endregion
    }
}