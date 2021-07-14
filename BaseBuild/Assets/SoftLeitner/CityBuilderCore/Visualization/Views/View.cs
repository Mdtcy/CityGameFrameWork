using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// base class for views<br/>
    /// views are ways to show additional information in game that can be activated<br/>
    /// for example bars over buildings, different camera cullings, overlays on tilemaps, ...
    /// </summary>
    public abstract class View : ScriptableObject
    {
        public string Name;

        public abstract void Activate();
        public abstract void Deactivate();
    }
}