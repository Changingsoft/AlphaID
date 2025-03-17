using Microsoft.Extensions.DependencyInjection;

namespace RadiusCore.Tests;

public class ServiceProviderFixture
{
    public ServiceProviderFixture()
    {
        var services = new ServiceCollection();

        services.AddOptions();
        services.AddLogging();
        services.AddRadiusCore();

        RootServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}