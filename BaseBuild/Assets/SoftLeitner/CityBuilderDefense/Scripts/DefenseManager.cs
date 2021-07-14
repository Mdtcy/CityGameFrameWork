using CityBuilderCore;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CityBuilderDefense
{
    public class DefenseManager : MonoBehaviour
    {
        private static bool _isStarted = false;

        public GameObject StartObject;
        public GameObject WinObject;
        public GameObject LoseObject;

        public TilemapSpawner Spawner;
        public DefenseStage[] Stages;

        private bool _hasWon;

        void Start()
        {
            this.StartChecker(() => checkStage(false));

            Dependencies.Get<IBuildingManager>().Deregistered += buildingDeregistered;

            if (!_isStarted)
            {
                ShowMission();
                _isStarted = true;
            }
        }

        public void LoadingChanged(bool isLoading)
        {
            if (isLoading)//loading has started
                _hasWon = false;
            else//loading is done
                checkStage(true);
        }

        public void ShowMission()
        {
            Dependencies.Get<IGameSpeed>().Pause();
            StartObject.SetActive(true);
        }
        public void StartMission()
        {
            Dependencies.Get<IGameSpeed>().Resume();
            StartObject.SetActive(false);
        }

        public void Recalculate()
        {
            StartCoroutine(recalculate());
        }
        private IEnumerator recalculate()
        {
            yield return null;

            foreach (var attackWalker in Spawner.GetComponentsInChildren<AttackWalker>())
            {
                if (!attackWalker)
                    continue;

                attackWalker.Recalculate();

                yield return null;
            }
        }

        private bool checkStage(bool isLoading)
        {
            var playtime = Dependencies.Get<IGameSpeed>().Playtime;

            var started = Stages.Where(s => s.StartTime < playtime).ToList();

            if (started.Count == Stages.Length)
            {
                if (!isLoading && !_hasWon)
                    win();
                _hasWon = true;
                return false;
            }
            else
            {
                Spawner.Interval = started.Last().SpawnInterval;
                return true;
            }
        }

        private void buildingDeregistered(IBuilding obj)
        {
            if (Dependencies.Get<IGameSaver>().IsLoading)
                return;

            if (!Dependencies.Get<IBuildingManager>().GetBuildingTraits<IAttackable>().Any())
                lose();
        }

        private void win()
        {
            Dependencies.Get<IGameSpeed>().Pause();
            WinObject.SetActive(true);
        }

        private void lose()
        {
            Dependencies.Get<IGameSpeed>().Pause();
            LoseObject.SetActive(true);
        }

        public void Continue()
        {
            WinObject.SetActive(false);
            LoseObject.SetActive(false);
            Dependencies.Get<IGameSpeed>().Resume();
        }

        public void Retry()
        {
            Dependencies.Get<IGameSpeed>().Resume();
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
}