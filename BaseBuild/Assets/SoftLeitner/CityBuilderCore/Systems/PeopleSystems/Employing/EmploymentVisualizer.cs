using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// visualizes employment as text in unity ui<br/>
    /// </summary>
    public class EmploymentVisualizer : MonoBehaviour
    {
        public Population Population;
        public TMPro.TMP_Text Text;

        private IEmploymentManager _employmentManager;

        private void Start()
        {
            _employmentManager = Dependencies.Get<IEmploymentManager>();
        }

        private void Update()
        {
            Text.text = $"{Population.Name}: {_employmentManager.GetAvailable(Population)} / {_employmentManager.GetNeeded(Population)}";
        }
    }
}