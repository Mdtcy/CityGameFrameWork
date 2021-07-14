using UnityEngine.SceneManagement;

namespace CityBuilderCore
{
    /// <summary>
    /// dialog for mission stuff<br/>
    /// just a wrapper for <see cref="MissionVisualizer"/>
    /// </summary>
    public class MissionDialog : DialogBase
    {
        public MissionVisualizer MissionVisualizer;
        public string ExitSceneName;

        public override void Activate()
        {
            base.Activate();

            MissionVisualizer.Mission = Dependencies.Get<IMissionManager>().MissionParameters.Mission;
            MissionVisualizer.UpdateVisuals();
        }

        public void ExitMission()
        {
            SceneManager.LoadSceneAsync(ExitSceneName);
        }
    }
}