namespace CityBuilderCore
{
    /// <summary>
    /// building components that have risks that can be reduced by a <see cref="RiskWalker"/> or are otherwise executed
    /// </summary>
    public interface IRiskRecipient : IBuildingComponent
    {
        RiskRecipient[] RiskRecipients { get; }

        bool HasRiskValue(Risk risk);
        float GetRiskValue(Risk risk);
        void ReduceRisk(Risk risk, float amount);
    }
}