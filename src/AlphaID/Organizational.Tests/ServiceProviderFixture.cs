using Microsoft.Extensions.DependencyInjection;

namespace Organizational.Tests;
public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        IServiceCollection services = new ServiceCollection();
        var builder = services.AddOrganizational();
        builder.AddOrganizationStore<StubOrganizationStore>();

        ServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = ServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }
    public ServiceProvider ServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}
