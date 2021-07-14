using UnityEngine;

namespace CityBuilderCore
{
    public class GameSaverProxy : MonoBehaviour, IGameSaver
    {
        public bool IsLoading => Dependencies.Get<IGameSaver>().IsLoading;
        public bool IsSaving => Dependencies.Get<IGameSaver>().IsSaving;

        public void Load()=> Dependencies.Get<IGameSaver>().Load();
        public void Save()=> Dependencies.Get<IGameSaver>().Save();
    }
}
