using Microsoft.Extensions.DependencyInjection;
using RadiusCore.Packet;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Xunit;

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
        var request = "010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003";
        var expected = "0200002686fe220e7624ba2a1005f6bf9b55e0b20606000000010f06000000000e06c0a80103";
        var secret = "xyzzy5461";


    }


    /// <summary>
    /// Test status-server response
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacket()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
        var expected = "02da0014ef0d552a4bf2d693ec2b6fe8b5411d66";
        var secret = "xyzzy5461";
    }


    /// <summary>
    /// Test status-server response
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAccountingResponsePacket()
    {
        var request = "0cb30026925f6b66dd5fed571fcb1db7ad3882605012e8d6eabda910875cd91fdade26367858";
        var expected = "05b300140f6f92145f107e2f504e860a4860669c";  // Note the error in the RFC. First byte should be 05 not 02
        var secret = "xyzzy5461";

    }


    /// <summary>
    /// Send test packet and verify response packet with proxy-state (21053135342105323330)
    /// Example from https://tools.ietf.org/html/rfc2865
    /// Modified to include two proxy states at the end
    /// </summary>
    [Fact]
    public void TestResponsePacketWithProxyState()
    {
        var request = "010000420f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a8011005060000000321053135342105323330";
        var expected = "02000030acf049cee1a3ed134316e5b3348cdf3c0606000000010f06000000000e06c0a8010321053135342105323330";
        var secret = "xyzzy5461";

    }


    /// <summary>
    /// Send test packet and verify response packet with proxy-state (21053135342105323330)
    /// Example from https://tools.ietf.org/html/rfc2865
    /// Modified to include two proxy states in the middle
    /// </summary>
    [Fact]
    public void TestResponsePacketWithProxyStateMiddle()
    {
        var request = "010000420f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a8011021053135342105323330050600000003";
        var expected = "02000030acf049cee1a3ed134316e5b3348cdf3c0606000000010f06000000000e06c0a8010321053135342105323330";
        var secret = "xyzzy5461";

    }


    /// <summary>
    /// Test 3GPP location info parsing from authentication packet
    /// </summary>
    [Fact]
    public void Test3GppLocationInfoParsing()
    {
        var request = "01d901becff27ef45a6bc4525aa5d65f483876951f103433363838383735303636393736011b32333230313038353030373639373640666c6578696e65747304060af7e0611a1600001fe40120001031352e3020283537393333290606000000020706000000073d06000000121a0e00001fe4003e0008000000011a17000028af01113233323031303835303037363937361a0d000028af080732333230311a09000028af0a03351a09000028af0c03301a0c000028af020605b28a3e1a27000028af052130352d314239333146373339364645464537344142464646463030384530301a0c000028af0d06303830301e0b666c6578696e6574731a0c000028af0606c230840c1a0d000028af120732333230311a0c000028af0706c23084da1a0d000028af090732333230311a09000028af1a03001a09000028af1503011a10000028af160a0132f210426d1bc01a0a000028af170480011a18000028af1412383632383238303231323838323230301a0c000028af0306000000001a0e00001fe4001800080000000405060001272602120ca8378c51ef621ac229c647a85646071a1100000009170b464c4558494e45545321053136382105313736";
        var secret = "xyzzy5461";

        var radiusPacketParser = serviceProvider.RootServiceProvider.GetRequiredService<IRadiusPacketParser>();
        var requestPacket = radiusPacketParser.Parse(Utils.StringToByteArray(request));
        var locationInfo = requestPacket.GetAttribute<byte[]>("3GPP-User-Location-Info");

        Assert.Equal("23201", Utils.GetMccMncFrom3GPPLocationInfo(locationInfo!).mccmnc);
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
        var request = "017301cea4571e304078c73bbb4367ca5fcede3e1f103433363838383735303636393736011b32333230313038353030373639373640666c6578696e65747304060af7e0611a1600001fe40120001031352e3020283537393333291e0b666c6578696e6574730606000000020706000000073d06000000121a0e00001fe4003e0008000000021a17000028af01113233323031303835303037363937361a0d000028af080732333230311a09000028af0a03351a0c000028af020605654e411a0c000028af0d06303830301a0c000028af0606c23084e01a0c000028af0706c23084da1a09000028af1503061a18000028af1412383632383238303231323838323230301a15000028af160f8232f210426d32f21000013e021a0d000028af120732333230311a0d000028af090732333230311a09000028af0c03301a0a000028af170480011a1f000028af051930382d34433038303030343933453030303034393345301a0c000028af0306000000001a09000028af1a03001a0e00001fe4001800080000000f050600012d2202121e205a653bc6cad430e70a585ab0271f1a0c0000159f5806000000031a1100000009170b464c4558494e4554532104393521043935210439352105323136";
        var secret = "xyzzy5461";

        Utils.StringToByteArray("8232f210426d32f21000013e02");
        var radiusPacketParser = serviceProvider.RootServiceProvider.GetRequiredService<IRadiusPacketParser>();
        var requestPacket = radiusPacketParser.Parse(Utils.StringToByteArray(request));
        var locationInfo = requestPacket.GetAttribute<byte[]>("3GPP-User-Location-Info");
        Assert.Equal("23201", Utils.GetMccMncFrom3GPPLocationInfo(locationInfo!).mccmnc);
    }


    /// <summary>
    /// Test status-server response
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacketUdpClient()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
        var expected = "02da0014ef0d552a4bf2d693ec2b6fe8b5411d66";
        var secret = "xyzzy5461";



        serviceProvider.RootServiceProvider.GetRequiredService<IRadiusPacketParser>();
        var rs = serviceProvider.RootServiceProvider.GetRequiredService<RadiusServer>();
    }


    /// <summary>
    /// Test status-server response with IPAddress.Any
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacketUdpClientAny()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
        var expected = "02da0014ef0d552a4bf2d693ec2b6fe8b5411d66";
        var secret = "xyzzy5461";

    }


    /// <summary>
    /// Test status-server response with IPNetwork
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestStatusServerAuthenticationResponsePacketUdpClientNetwork()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
        var expected = "02da0014ef0d552a4bf2d693ec2b6fe8b5411d66";
        var secret = "xyzzy5461";

    }


    /// <summary>
    /// Test IPacketHandlerRepository interface
    /// Example from https://tools.ietf.org/html/rfc5997#section-6
    /// </summary>
    [Fact]
    public void TestPacketHandlerRepositoryInterface()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
        var expected = "02da0014ef0d552a4bf2d693ec2b6fe8b5411d66";
        var secret = "xyzzy5461";

    }
}