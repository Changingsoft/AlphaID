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
        //替换为MockUdpClient
        services.AddSingleton<IUdpClient, MockUdpClient>();


        RootServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}