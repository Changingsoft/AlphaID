namespace AlphaIdPlatform.Tests;
public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        IServiceCollection services = new ServiceCollection();
        var builder = services.AddAlphaIdPlatform();
        builder.AddOrganizationStore<StubOrganizationStore>();

        ServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = ServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }
    public ServiceProvider ServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}
