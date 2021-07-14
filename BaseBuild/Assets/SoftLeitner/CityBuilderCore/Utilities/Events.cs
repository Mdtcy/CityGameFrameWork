using System;
using UnityEngine;
using UnityEngine.Events;

namespace CityBuilderCore
{
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class IntEvent : UnityEvent<int> { }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Serializable]
    public class PointValueEvent : UnityEvent<Vector2Int, int> { }
}
