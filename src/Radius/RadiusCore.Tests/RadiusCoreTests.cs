using Microsoft.Extensions.Logging.Abstractions;
using RadiusCore.Dictionary;
using RadiusCore.Packet;
using System.Net;
using System.Text;

namespace RadiusCore.Tests;

public class RadiusCoreTests
{
    private static IRadiusDictionary GetDictionary() => RadiusDictionary.Parse(DefaultDictionary.RadiusDictionary);


    /// <summary>
    /// Create packet and verify bytes
    /// Example from https://tools.ietf.org/html/rfc2865
    /// </summary>
    [Fact]
    public void CreateAccessRequestPacket()
    {
        var packet = new RadiusPacket(PacketCode.AccessRequest, 0)
        {
            Authenticator = Utils.StringToByteArray("0f403f9473978057bd83d5cb98f4227a")
        };
        packet.AddAttribute("User-Name", "nemo");
        packet.AddAttribute("User-Password", "arctangent");
        packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("192.168.1.16"));
        packet.AddAttribute("NAS-Port", 3);
    }


    /// <summary>
    /// Create packet and verify bytes, including IPv6 attribute
    /// Example from https://tools.ietf.org/html/rfc2865
    /// </summary>
    [Fact]
    public void TestCreateAccessRequestPacketIPv6()
    {
        var expected = IPAddress.IPv6Loopback;

        var packet = new RadiusPacket(PacketCode.AccessRequest, 0)
        {
            Authenticator = Utils.StringToByteArray("0f403f9473978057bd83d5cb98f4227a")
        };
        packet.AddAttribute("User-Name", "nemo");
        packet.AddAttribute("User-Password", "arctangent");
        packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("192.168.1.16"));
        packet.AddAttribute("Framed-IPv6-Address", expected);
        packet.AddAttribute("NAS-Port", 3);

        var actual = packet.GetAttribute<IPAddress>("Framed-IPv6-Address");
        Assert.Equal(expected, actual);
    }


    /// <summary>
    /// Create packet and verify bytes
    /// Example from https://tools.ietf.org/html/rfc2865
    /// </summary>
    [Fact]
    public void TestCreateAccessRequestPacketUnknownAttribute()
    {
        var packet = new RadiusPacket(PacketCode.AccessRequest, 0)
        {
            Authenticator = Utils.StringToByteArray("0f403f9473978057bd83d5cb98f4227a")
        };
        packet.AddAttribute("User-Name", "nemo");
        packet.AddAttribute("hurr", "durr");
        packet.AddAttribute("User-Password", "arctangent");
        packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("192.168.1.16"));
        packet.AddAttribute("NAS-Port", 3);
    }


    /// <summary>
    /// Create disconnect request packet and verify bytes
    /// </summary>
    [Fact]
    public void TestCreateDisconnectRequestPacket()
    {
        var packet = new RadiusPacket(PacketCode.DisconnectRequest, 1);
        packet.AddAttribute("Acct-Session-Id", "09042AF8");
    }


    /// <summary>
    /// Create status server request packet and verify bytes
    /// </summary>
    [Fact]
    public void TestCreateStatusServerRequestPacket()
    {
        var packet = new RadiusPacket(PacketCode.StatusServer, 218)
        {
            Authenticator = Utils.StringToByteArray("8a54f4686fb394c52866e302185d0623")
        };
    }


    /// <summary>
    /// Create status server request packet and verify bytes
    /// </summary>
    [Fact]
    public void TestCreateStatusServerRequestPacketAccounting()
    {
        var packet = new RadiusPacket(PacketCode.StatusServer, 179)
        {
            Authenticator = Utils.StringToByteArray("925f6b66dd5fed571fcb1db7ad388260")
        };
    }


    /// <summary>
    /// Create accounting request        
    /// </summary>
    [Fact]
    public void TestCreateAndParseAccountingRequestPacket()
    {
        GetDictionary();
        var packet = new RadiusPacket(PacketCode.AccountingRequest, 0);
        packet.AddAttribute("User-Name", "nemo");
        packet.AddAttribute("Acct-Status-Type", 2);
        packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("192.168.1.16"));
        packet.AddAttribute("NAS-Port", 3);
    }


    ///// <summary>
    ///// Create packet and verify bytes
    ///// Example from https://tools.ietf.org/html/rfc2865
    ///// </summary>
    [Fact]
    public void TestAccountingPacketRequestAuthenticatorSuccess()
    {
        var packetBytes = "0404002711019c27d4e00cbc523b3e2fc834baf401066e656d6f2806000000012c073230303234";
        var secret = "xyzzy5461";

        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        var requestAuthenticator = Utils.CalculateRequestAuthenticator(
            Encoding.UTF8.GetBytes(secret),
            Utils.StringToByteArray(packetBytes));
        var packet = radiusPacketParser.Parse(Utils.StringToByteArray(packetBytes));

        Assert.Equal(packet.Authenticator.ToHexString(), requestAuthenticator.ToHexString());
    }


    ///// <summary>
    ///// Create packet and verify bytes
    ///// Example from https://tools.ietf.org/html/rfc2865
    ///// </summary>
    [Fact]
    public void TestAccountingPacketRequestAuthenticatorFail()
    {
        var packetBytes = "0404002711019c27d4e00cbc523b3e2fc834baf401066e656d6f2806000000012c073230303234";
        var secret = "foo";

        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        Utils.CalculateRequestAuthenticator(
            Encoding.UTF8.GetBytes(secret),
            Utils.StringToByteArray(packetBytes));
        Assert.Throws<InvalidOperationException>(
            () => radiusPacketParser.Parse(Utils.StringToByteArray(packetBytes)));
    }


    /// <summary>
    /// Test parsing and rebuilding a packet
    /// </summary>
    [Fact]
    public void TestPacketParserAndAssembler()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";
        var expected = request;


        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        var requestPacket = radiusPacketParser.Parse(Utils.StringToByteArray(request));
    }

    /// <summary>
    /// Test parsing and rebuilding a packet
    /// </summary>
    [Fact]
    public void TestPacketParserAndAssemblerExtraDataIgnored()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa300ff00ff00ff";


        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        var requestPacket = radiusPacketParser.Parse(Utils.StringToByteArray(request));
    }


    /// <summary>
    /// Test parsing packet with missing data
    /// </summary>
    [Fact]
    public void TestPacketParserMissingData()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84f";

        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            radiusPacketParser.Parse(Utils.StringToByteArray(request)));
    }


    /// <summary>
    /// Test parsing and rebuilding a packet
    /// </summary>
    [Fact]
    public void TestCreatingAndParsingPacket()
    {
        var packet = new RadiusPacket(PacketCode.AccessRequest, 1);
        packet.AddAttribute("User-Name", "test@example.com");
        packet.AddAttribute("User-Password", "test");
        packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("127.0.0.1"));
        packet.AddAttribute("NAS-Port", 100);
        packet.AddAttribute("3GPP-IMSI-MCC-MNC", "24001");
        packet.AddAttribute("3GPP-CG-Address", IPAddress.Parse("127.0.0.1"));

    }


    /// <summary>
    /// Test parsing and rebuilding a packet
    /// </summary>
    [Fact]
    public void TestCreatingMissingAttributes()
    {
        var packet = new RadiusPacket(PacketCode.AccessRequest, 1);
        packet.AddAttribute("User-Name", "test@example.com");
        packet.AddAttribute("User-Password", "test");
    }


    /// <summary>
    /// Test message authenticator validation success
    /// </summary>
    [Fact]
    public void TestMessageAuthenticatorValidationSuccess()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";

        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        radiusPacketParser.Parse(Utils.StringToByteArray(request));
    }


    /// <summary>
    /// Test message authenticator validation fail
    /// </summary>
    [Fact]
    public void TestMessageAuthenticatorValidationFail()
    {
        var request = "0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3";

        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        Assert.Throws<MessageAuthenticatorException>(() => radiusPacketParser.Parse(Utils.StringToByteArray(request)));
    }


    /// <summary>
    /// Test passwords with length > 16        
    /// </summary>
    [Theory]
    [InlineData("123456789")]
    [InlineData("12345678901234567890")]
    public void TestPasswordEncryptDecrypt(string password)
    {
        var secret = "xyzzy5461";
        var authenticator = "1234567890123456";

        var encrypted = RadiusPassword.Encrypt(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(authenticator),
            Encoding.UTF8.GetBytes(password));

        var decrypted = RadiusPassword.Decrypt(Encoding.UTF8.GetBytes(secret), Encoding.UTF8.GetBytes(authenticator),
            encrypted);


        Assert.Equal(password, decrypted);
    }


    /// <summary>
    /// Create CoA request packet and verify bytes
    /// </summary>
    [Fact]
    public void TestCreateCoARequestPacket()
    {
        var packet = new RadiusPacket(PacketCode.CoaRequest, 0)
        {
            Authenticator = Utils.StringToByteArray("0f403f9473978057bd83d5cb98f4227a")
        };
        packet.AddAttribute("User-Name", "nemo");
        packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("192.168.1.16"));
        packet.AddAttribute("NAS-Port", 3);
    }


    /// <summary>
    /// Test message authenticator validation success with no side effect
    /// </summary>
    [Fact]
    public void TestMessageAuthenticatorNoSideEffect()
    {
        var request =
            Utils.StringToByteArray("0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3");
        var expected =
            Utils.StringToByteArray("0cda00268a54f4686fb394c52866e302185d062350125a665e2e1e8411f3e243822097c84fa3");

        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);
        radiusPacketParser.Parse(request);
        Assert.Equal(expected.ToHexString(), request.ToHexString());
    }


    [Fact]
    public void TestMessageAuthenticatorResponsePacket()
    {
        var response = new RadiusPacket(PacketCode.AccessReject, 104)
        {
        };

        response.AddAttribute("EAP-Message", Utils.StringToByteArray("04670004"));
        response.AddMessageAuthenticator();
    }

    [Fact]
    public void TestMessageAuthenticatorResponsePacketBlastRadius()
    {
        // access accept response packet
        var response =
            Utils.StringToByteArray("020000261b49188b89251f7c9b8604772ca685925012b02cae7428c0e4e2301c060a5bf75bff");

        // request authenticator from the corresponding request
        var requestAuthenticator = Utils.StringToByteArray("fb421846209424ca0982ad9326e5ccf0");
        Utils.StringToByteArray("020000261b49188b89251f7c9b8604772ca685925012b02cae7428c0e4e2301c060a5bf75bff");

        var radiusPacketParser = new RadiusRequestParser(GetDictionary(), NullLogger<RadiusRequestParser>.Instance);

        radiusPacketParser.Parse(response);
    }

    [Fact]
    public void TestMessageAuthenticatorResponsePacketBlastRadiusMissingAuthenticator()
    {
        // access accept response packet
        var response =
            Utils.StringToByteArray(
                "010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003");

        // request authenticator from the corresponding request
        var requestAuthenticator = Utils.StringToByteArray("fb421846209424ca0982ad9326e5ccf0");
        Utils.StringToByteArray("020000261b49188b89251f7c9b8604772ca685925012b02cae7428c0e4e2301c060a5bf75bff");

    }

    /// <summary>
    /// Test message authenticator validation success with no side effect
    /// </summary>
    [Fact]
    public void TestVendorSpecificAttribute()
    {
        // "3GPP-IMSI-MCC-MNC": "24001"
        var bytes = Utils.StringToByteArray("000028af08073234303031");

        var vsa = new VendorSpecificAttribute(bytes);

        Assert.Multiple(() =>
        {
            Assert.Equal((uint)10415, vsa.VendorId);
            Assert.Equal(8, vsa.VendorCode);
            Assert.Equal("3234303031", vsa.Value.ToHexString());
            Assert.Equal(7, vsa.Length);
        });
    }
}