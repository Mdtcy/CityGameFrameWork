using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for scores used for win conditions and statistics
    /// </summary>
    public abstract class Score : ScriptableObject
    {
        public string Name;

        public abstract int Calculate();
    }
}