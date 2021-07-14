using System;

namespace CityBuilderCore
{
    public interface IConnectionFeeder : IConnectionPasser
    {
        int Value { get; }
        int Range { get; }
        int Falloff { get; }

        event Action<IConnectionFeeder> FeederValueChanged;
    }
}
