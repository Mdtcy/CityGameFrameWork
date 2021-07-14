using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// a type of worker(mason, carpenter, laborer)
    /// </summary>
    [CreateAssetMenu(menuName = "CityBuilder/" + nameof(Worker))]
    public class Worker : ScriptableObject
    {
        public string Name;
        public Sprite Icon;
    }
}