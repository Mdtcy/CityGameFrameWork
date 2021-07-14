using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// any object that has health wich should be displayed
    /// </summary>
    public interface IHealther
    {
        float TotalHealth { get; }
        float CurrentHealth { get; }
        Vector3 HealthPosition { get; }
    }
}