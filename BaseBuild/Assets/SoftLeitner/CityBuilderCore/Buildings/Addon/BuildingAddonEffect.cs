using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// blank building addon that can be used to attach particle effects for example<br/>
    /// removal is either done when the building is replaced(Evolution went through) or from the outside(evolution canceled)
    /// </summary>
    public class BuildingAddonEffect : BuildingAddon
    {
        public bool RemoveOnReplace;

        public override void OnReplacing(Transform parent, IBuilding replacement)
        {
            if (!RemoveOnReplace)
                base.OnReplacing(parent, replacement);
        }
    }
}