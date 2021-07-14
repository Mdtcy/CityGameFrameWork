using System;

namespace CityBuilderCore
{
    /// <summary>
    /// walks leaving population out of the map
    /// </summary>
    public class EmigrationWalker : Walker
    {
        public int Capacity;

        public void StartEmigrating(WalkingPath path)
        {
            walk(path);
        }
    }

    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class ManualEmigrationWalkerSpawner : ManualWalkerSpawner<EmigrationWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class CyclicEmigrationWalkerSpawner : CyclicWalkerSpawner<EmigrationWalker> { }
    /// <summary>
    /// concrete implementation for serialization, not needed starting unity 2020.1
    /// </summary>
    [Serializable]
    public class PooledEmigrationWalkerSpawner : PooledWalkerSpawner<EmigrationWalker> { }
}