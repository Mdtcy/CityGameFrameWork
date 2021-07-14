using UnityEngine;

namespace CityBuilderCore
{
    public class GameSpeedProxy : MonoBehaviour, IGameSpeed
    {
        public float Playtime => Dependencies.Get<IGameSpeed>().Playtime;

        public void Pause() => Dependencies.Get<IGameSpeed>().Pause();
        public void Resume() => Dependencies.Get<IGameSpeed>().Resume();
        public void SetSpeed(float speed) => Dependencies.Get<IGameSpeed>().SetSpeed(speed);
    }
}
