namespace CityBuilderCore
{
    public interface IEfficiencyFactor
    {
        bool IsWorking { get; }
        float Factor { get; }
    }
}