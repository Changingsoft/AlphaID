using System.Net;
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
        services.AddSingleton<IUdpClient>(_ => new MockUdpClient(new IPEndPoint(IPAddress.Any, 1812), new IPEndPoint(IPAddress.Parse("192.168.1.5"), 54321)));


        RootServiceProvider = services.BuildServiceProvider();
        ServiceScopeFactory = RootServiceProvider.GetRequiredService<IServiceScopeFactory>();
    }

    public IServiceProvider RootServiceProvider { get; }

    public IServiceScopeFactory ServiceScopeFactory { get; }
}