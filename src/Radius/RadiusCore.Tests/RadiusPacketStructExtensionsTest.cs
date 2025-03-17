using RadiusCore.Packet;
using System.Net;
using Xunit;

namespace RadiusCore.Tests;

public class RadiusPacketStructExtensionsTest
{
    [Fact]
    public void ToByteArrayTest()
    {
        var packet = new RadiusPacketDataStruct
        {
            Code = 1,
            Identifier = 2,
            Length = 26,
            Authenticator = new byte[16]
        };
        List<RadiusAttributeStruct> attributes = [new RadiusAttributeStruct { Length = 6, Type = 5, ValuePtr = (new byte[] { 0x13, 0x25, 0x11, 0x25 }).ToPtr() }];
        var bytes = packet.ToByteArray(attributes);
        Assert.Equal(1, bytes[0]);
        Assert.Equal(2, bytes[1]);

        // 将 Length 转换为网络字节序
        ushort networkOrderLength = (ushort)IPAddress.HostToNetworkOrder((short)packet.Length);
        Assert.Equal(networkOrderLength, BitConverter.ToUInt16(bytes[2..4]));

        Assert.Equal(16, bytes[4..20].Length);
    }

    [Fact]
    public void FromByteArrayTest()
    {
        byte[] data = [1, 2, 0, 26, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 6, 13, 25, 11, 25];

        var packet = RadiusPacketDataStructExtensions.FromByteArray(data, out var attributes);
        Assert.Equal(1, packet.Code);
        Assert.Equal(2, packet.Identifier);
        Assert.Equal(26, packet.Length);
        Assert.Equal(16, packet.Authenticator.Length);
        var attribute = attributes.First();
        Assert.Equal(5, attribute.Type);
        Assert.Equal(6, attribute.Length);
        Assert.Equal(4, attribute.GetValue().Length);
    }
}