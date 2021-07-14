using UnityEngine;

namespace CityBuilderCore
{
    public enum DiseaseMortality
    {
        [Tooltip("population is not reduced by the disease")]
        None = 0,
        [Tooltip("when disease hits a ratio of the housing population is immediately killed")]
        Initial = 10,
        [Tooltip("housing population is continuously killed at a certain rate/sec")]
        Final = 11,
        [Tooltip("a ratio of the housing population is killed of when the duration of the disease is over")]
        Continuous = 20
    }
}