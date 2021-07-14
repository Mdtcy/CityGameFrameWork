namespace CityBuilderCore
{
    /// <summary>
    /// building components that have services that can be filled by a <see cref="RiskWalker"/>
    /// </summary>
    public interface IServiceRecipient : IBuildingComponent
    {
        ServiceRecipient[] ServiceRecipients { get; }

        void ProvideService(Service service, float amount);
        bool HasServiceValue(Service service);
        float GetServiceValue(Service service);
    }
}