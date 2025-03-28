using System.Net;
using Microsoft.Extensions.DependencyInjection;
using RadiusCore.Packet;

namespace RadiusCore.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RadiusServerTests(ServiceProviderFixture serviceProvider)
{
    [Fact]
    public async Task SendAccessRequest()
    {
        var udp = serviceProvider.RootServiceProvider.GetRequiredService<IUdpClient>().GetMockUdpClient();
        var server = serviceProvider.RootServiceProvider.GetRequiredService<RadiusServer>();
        await server.StartAsync(CancellationToken.None);

        await udp.RemoteClient.SendAsync(new ReadOnlyMemory<byte>(Utils.StringToByteArray("010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003")), new IPEndPoint(IPAddress.Any, 1812),
            CancellationToken.None);

        var result = await udp.RemoteClient.ReceiveAsync(CancellationToken.None);
        RadiusPacket responsePacket = RadiusPacket.FromByteArray(result.Buffer);
        Assert.Equal(PacketCode.AccessAccept, responsePacket.Code);
    }
}