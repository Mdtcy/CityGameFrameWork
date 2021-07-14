using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// displays the current playtime in unity ui
    /// </summary>
    public class PlaytimeVisual : MonoBehaviour
    {
        public TMPro.TMP_Text Text;
        public string Format;

        private IGameSpeed _gameSpeed;

        private void Start()
        {
            _gameSpeed = Dependencies.Get<IGameSpeed>();
        }

        private void Update()
        {
            Text.text = _gameSpeed.Playtime.ToString(Format);
        }
    }
}