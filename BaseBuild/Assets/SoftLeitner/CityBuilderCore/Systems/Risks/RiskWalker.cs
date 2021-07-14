using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// roams around and reduces the risks of <see cref="IRiskRecipient"/> while it is in range
    /// </summary>
    public class RiskWalker : BuildingComponentWalker<IRiskRecipient>
    {
        public Risk Risk;
        [Tooltip("risk reduction per second")]
        public float Amount = 100f;

        protected override void onComponentRemaining(IRiskRecipient risker)
        {
            base.onComponentEntered(risker);

            risker.ReduceRisk(Risk, Amount * Time.deltaTime);
        }
    }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class ManualRiskWalkerSpawner : ManualWalkerSpawner<RiskWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class CyclicRiskWalkerSpawner : CyclicWalkerSpawner<RiskWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class PooledRiskWalkerSpawner : PooledWalkerSpawner<RiskWalker> { }
}