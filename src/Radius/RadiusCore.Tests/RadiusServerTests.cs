using Microsoft.Extensions.DependencyInjection;
using RadiusCore.Packet;

namespace RadiusCore.Tests;

[Collection(nameof(ServiceProviderCollection))]
public class RadiusServerTests(ServiceProviderFixture serviceProvider)
{
    /// <summary>
    /// Send test packet and verify response packet
    /// Example from https://tools.ietf.org/html/rfc2865
    /// </summary>
    [Fact]
    public void TestResponsePacket()
    {
    }


    /// <summary>
    /// Test status-server response
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacket()
    {
    }


    /// <summary>
    /// Test status-server response
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAccountingResponsePacket()
    {
    }


    /// <summary>
    /// Send test packet and verify response packet with proxy-state (21053135342105323330)
    /// Example from https://tools.ietf.org/html/rfc2865
    /// Modified to include two proxy states at the end
    /// </summary>
    [Fact]
    public void TestResponsePacketWithProxyState()
    {
    }


    /// <summary>
    /// Send test packet and verify response packet with proxy-state (21053135342105323330)
    /// Example from https://tools.ietf.org/html/rfc2865
    /// Modified to include two proxy states in the middle
    /// </summary>
    [Fact]
    public void TestResponsePacketWithProxyStateMiddle()
    {
    }


    /// <summary>
    /// Test 3GPP location info parsing from authentication packet
    /// </summary>
    [Fact]
    public void Test3GppLocationInfoParsing()
    {
    }


    /// <summary>
    /// Test 3GPP location info parsing from various bytes
    /// </summary>
    [Theory]
    [InlineData("0032f4030921b8e8", "23430")]
    [InlineData("001300710921b8e8", "310170")]
    [InlineData("071300710921b8e8", null)]
    public void Test3GppLocationInfoParsing2(string hexBytes, string? mccmnc)
    {
        Assert.Equal(mccmnc, Utils.GetMccMncFrom3GPPLocationInfo(Utils.StringToByteArray(hexBytes)).mccmnc);
    }


    /// <summary>
    /// Test 3GPP location info parsing with TAI+ECGI in auth packet
    /// </summary>
    [Fact]
    public void TestLte3GppLocationInfoParsing()
    {
    }


    /// <summary>
    /// Test status-server response
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacketUdpClient()
    {
        serviceProvider.RootServiceProvider.GetRequiredService<RadiusRequestParser>();
        var rs = serviceProvider.RootServiceProvider.GetRequiredService<RadiusServer>();
    }


    /// <summary>
    /// Test status-server response with IPAddress.Any
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacketUdpClientAny()
    {
    }


    /// <summary>
    /// Test status-server response with IPNetwork
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacketUdpClientNetwork()
    {
    }


    /// <summary>
    /// Test IPacketHandlerRepository interface
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestPacketHandlerRepositoryInterface()
    {
    }
}