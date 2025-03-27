using Microsoft.Extensions.DependencyInjection;
using RadiusCore.Packet;

namespace RadiusCore.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RadiusServerTests(ServiceProviderFixture serviceProvider)
{
    [Fact]
    public void SendAccessRequest()
    {
        var udp = serviceProvider.RootServiceProvider.GetRequiredService<IUdpClient>().GetMockUdpClient();
        var server = serviceProvider.RootServiceProvider.GetRequiredService<RadiusServer>();


    }
}