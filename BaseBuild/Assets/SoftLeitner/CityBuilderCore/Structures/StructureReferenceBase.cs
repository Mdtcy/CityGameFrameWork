using System;

namespace CityBuilderCore
{
    public abstract class StructureReferenceBase<T>
    where T : IStructure
    {
        public T Structure { get; private set; }

        public event Action<T, T> Replacing;

        public StructureReferenceBase(T structure)
        {
            Structure = structure;
        }

        public virtual void Replace(T replacement)
        {
            Replacing?.Invoke(Structure, replacement);
            Structure = replacement;
        }
    }
}