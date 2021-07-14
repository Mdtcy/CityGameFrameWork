using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for entites moving about the map
    /// </summary>
    public abstract class Walker : MonoBehaviour, ISaveData, IOverrideHeight
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public RoamingState CurrentRoaming { get; set; }
        public WalkingState CurrentWalking { get; set; }
        public WaitingState CurrentWaiting { get; set; }

        public float? HeightOverride { get; set; }

        public WalkerInfo Info;
        [Tooltip("transform used in rotation and variance, should contain all visuals")]
        public Transform Pivot;

        public PathType PathType;
        public UnityEngine.Object PathTag;
        public float Speed = 5;
        public float MaxWait = 10f;
        public float Delay;

        public float TimePerStep => 1 / Speed;
        public Vector2Int CurrentPoint => _current;
        public virtual ItemStorage ItemStorage => null;

        private bool _isWalking;
        public bool IsWalking
        {
            get { return _isWalking; }
            set
            {
                _isWalking = value;
                IsWalkingChanged?.Invoke(value);
            }
        }
        public BoolEvent IsWalkingChanged;

        public event Action<Walker> Finished;
        public event Action<Walker> Moved;

        protected Vector2Int _start;
        protected Vector2Int _current;
        protected BuildingReference _home;

        private bool _isLoaded;

        /// <summary>
        /// called right after instantiating or reactivating a walker<br/>
        /// buildings have not had a chance to interact with the walker<br/>
        /// when your logic somthing from outside first override <see cref="Spawned"/> instead
        /// </summary>
        /// <param name="home"></param>
        /// <param name="start"></param>
        public virtual void Initialize(BuildingReference home, Vector2Int start)
        {
            _start = start;
            _current = start;
            _home = home;
        }

        /// <summary>
        /// called after the walker is fully initialized and its spawning has been signaled to the owner
        /// </summary>
        public virtual void Spawned() { }

        protected virtual void Start()
        {
            if (!_isLoaded && Pivot)
                Pivot.localPosition += Dependencies.Get<IMap>().GetVariance();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            try
            {
                string debugText = GetDebugText();
                if (string.IsNullOrWhiteSpace(debugText))
                    return;

                UnityEditor.Handles.Label(Pivot.position, debugText);
            }
            catch
            {
                //dont care
            }
        }
#endif

        private void OnDrawGizmosSelected()
        {
            if (CurrentWalking?.Path != null)
            {
                Gizmos.color = Color.white;

                for (int i = 0; i < CurrentWalking.Path.Length - 1; i++)
                {
                    Gizmos.DrawLine(CurrentWalking.Path.GetPreviousPosition(i), CurrentWalking.Path.GetNextPosition(i));
                }
            }
        }

        protected virtual void onMoved(Vector2Int position)
        {
            _current = position;
            Moved?.Invoke(this);
        }

        protected virtual void onFinished()
        {
            Finished?.Invoke(this);
            if (gameObject && gameObject.activeSelf)
                Destroy(gameObject);//in case walker was not released by spawner
        }

        protected void roam(int memoryLength, int range, Action finished = null)
        {
            StartCoroutine(WalkingPath.Roam(this, Delay, _current, memoryLength, range, finished ?? onFinished, onMoved));
        }
        protected void roam(float delay, int memoryLength, int range, Action finished = null)
        {
            StartCoroutine(WalkingPath.Roam(this, delay, _current, memoryLength, range, finished ?? onFinished, onMoved));
        }
        protected void continueRoam(int memoryLength, int range, Action finished)
        {
            StartCoroutine(WalkingPath.ContinueRoam(this, memoryLength, range, finished ?? onFinished, onMoved));
        }

        protected bool walk(IBuilding target,Action finished = null)
        {
            var path = PathHelper.FindPath(_current, target, PathType, PathTag);
            if (path == null)
                return false;
            walk(path, finished);
            return true;
        }
        protected void walk(WalkingPath path, Action finished = null)
        {
            StartCoroutine(path.Walk(this, Delay, finished ?? onFinished, onMoved));
        }
        protected void walk(WalkingPath path, float delay, Action finished = null)
        {
            StartCoroutine(path.Walk(this, delay, finished ?? onFinished, onMoved));
        }
        protected void continueWalk(Action finished = null)
        {
            StartCoroutine(CurrentWalking.Path.ContinueWalk(this, finished ?? onFinished, onMoved));
        }

        protected void wander(Action finished)
        {
            var adjacent = PathHelper.GetAdjacent(CurrentPoint, PathType, PathTag);
            if (!adjacent.Any())
            {
                finished();
                return;
            }

            walk(new WalkingPath(new[] { CurrentPoint, adjacent.Random() }), finished);
        }
        protected void continueWander(Action finished)
        {
            continueWalk(finished);
        }

        protected void wait(Action finished, float time)
        {
            StartCoroutine(waitRoutine(finished, time));
        }
        protected void delay(Action finished)
        {
            StartCoroutine(waitRoutine(finished, Delay));
        }
        private IEnumerator waitRoutine(Action finished, float time)
        {
            CurrentWaiting = new WaitingState() { SetTime = time };
            yield return continueWaitRoutine(finished);
        }
        protected void continueWait(Action finished)
        {
            StartCoroutine(continueWaitRoutine(finished));
        }
        private IEnumerator continueWaitRoutine(Action finished)
        {
            yield return CurrentWaiting.Wait();
            CurrentWaiting = null;
            finished();
        }

        protected void followPath(IEnumerable<Vector3> path, Action finished)
        {
            StartCoroutine(new WalkingPath(path.ToArray()).Walk(this, Delay, finished ?? onFinished));
        }
        protected void followPath(IEnumerable<Vector3> path, float delay, Action finished)
        {
            StartCoroutine(new WalkingPath(path.ToArray()).Walk(this, delay, finished ?? onFinished));
        }
        protected void continueFollow(Action finished)
        {
            StartCoroutine(CurrentWalking.Path.ContinueWalk(this, finished ?? onFinished));
        }

        protected void tryWalk(IBuilding target, float delay, Action planned = null, Action finished = null, Action canceled = null)
        {
            tryWalk(() => PathHelper.FindPath(_current, target, PathType, PathTag), delay, planned, finished, canceled);
        }
        protected void tryWalk(Vector2Int target, float delay, Action planned = null, Action finished = null, Action canceled = null)
        {
            tryWalk(() => PathHelper.FindPath(_current, target, PathType, PathTag), delay, planned, finished, canceled);
        }
        protected void tryWalk(Func<WalkingPath> pathGetter, float delay, Action planned = null, Action finished = null, Action canceled = null)
        {
            StartCoroutine(WalkingPath.TryWalk(this, delay, _current, pathGetter, planned: planned, finished: finished ?? onFinished, canceled: canceled, moved: onMoved));
        }

        protected void tryWalk(IBuilding target, Action planned = null, Action finished = null, Action canceled = null)
        {
            tryWalk(() => PathHelper.FindPath(_current, target, PathType, PathTag), planned, finished, canceled);
        }
        protected void tryWalk(Vector2Int target, Action planned = null, Action finished = null, Action canceled = null)
        {
            tryWalk(() => PathHelper.FindPath(_current, target, PathType, PathTag), planned, finished, canceled);
        }
        protected void tryWalk(Func<WalkingPath> pathGetter, Action planned = null, Action finished = null, Action canceled = null)
        {
            StartCoroutine(WalkingPath.TryWalk(this, Delay, _current, pathGetter, planned: planned, finished: finished ?? onFinished, canceled: canceled, moved: onMoved));
        }

        public void Hide()
        {
            Pivot.gameObject.SetActive(false);
            var collider = GetComponent<Collider2D>();
            if (collider)
                collider.enabled = false;
        }
        public void Show()
        {
            Pivot.gameObject.SetActive(true);
            var collider = GetComponent<Collider2D>();
            if (collider)
                collider.enabled = true;
        }

        public void Finish() => onFinished();

        public virtual string GetName() => Info.Name;
        public virtual string GetDescription() => Info.Descriptions.FirstOrDefault();
        public virtual string GetDebugText() => null;
        protected string getDescription(int index) => Info.Descriptions.ElementAtOrDefault(index);
        protected string getDescription(int index, params object[] parameters)
        {
            if (parameters == null)
                return getDescription(index);
            else
                return string.Format(getDescription(index), parameters);
        }

        #region Saving
        [Serializable]
        public class WalkerData
        {
            public string Id;
            public string HomeId;
            public Vector2Int StartPoint;
            public Vector2Int CurrentPoint;
            public Vector3 Position;
            public Vector3 PivotPosition;
            public Quaternion Rotation;
            public Quaternion PivotRotation;
            public RoamingState.RoamingData CurrentRoaming;
            public WalkingState.WalkingData CurrentWalking;
            public WaitingState.WaitingData CurrentWait;
            public float? HeightOverride;
        }

        public virtual string SaveData() => string.Empty;
        public virtual void LoadData(string json) { }

        protected WalkerData savewalkerData()
        {
            return new WalkerData()
            {
                Id = Id.ToString(),
                HomeId = _home?.Instance.Id.ToString(),
                StartPoint = _start,
                CurrentPoint = _current,
                Position = transform.position,
                Rotation = transform.rotation,
                PivotPosition = Pivot.localPosition,
                PivotRotation = Pivot.localRotation,
                CurrentWalking = CurrentWalking?.GetData(),
                CurrentRoaming = CurrentRoaming?.GetData(),
                CurrentWait = CurrentWaiting?.GetData(),
                HeightOverride = HeightOverride
            };
        }
        protected void loadWalkerData(WalkerData data)
        {
            _isLoaded = true;
            Id = new Guid(data.Id);
            if (!string.IsNullOrWhiteSpace(data.HomeId))
                _home = Dependencies.Get<IBuildingManager>().GetBuildingReference(new Guid(data.HomeId));
            _start = data.StartPoint;
            _current = data.CurrentPoint;
            transform.position = data.Position;
            transform.rotation = data.Rotation;
            Pivot.localPosition = data.PivotPosition;
            Pivot.localRotation = data.PivotRotation;
            CurrentRoaming = RoamingState.FromData(data.CurrentRoaming);
            CurrentWalking = WalkingState.FromData(data.CurrentWalking);
            CurrentWaiting = WaitingState.FromData(data.CurrentWait);
            HeightOverride = data.HeightOverride;
        }
        #endregion
    }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class WalkerEvent : UnityEvent<Walker> { }
}