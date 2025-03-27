using Microsoft.Extensions.Logging;
using System.Net;

namespace RadiusCore.Packet;

/// <summary>
/// RadiusRequestParser
/// </summary>
public class RadiusRequestParser(ILogger<RadiusRequestParser>? logger)
{
    /// <summary>
    /// Parses packet bytes and returns an IRadiusPacket
    /// </summary>
    public RadiusRequest Parse(byte[] packetBytes)
    {
        ReadOnlySpan<byte> packetBytesSpan = packetBytes;
        var indicatedLength = IPAddress.NetworkToHostOrder(BitConverter.ToUInt16(packetBytesSpan[2..4]));
        if (packetBytesSpan.Length < indicatedLength)
        {
            logger?.LogError("收到的数据包长度小于预期。数据包长度是{ByteArrayLength}，预期长度是{PacketLength}。", packetBytes.Length, indicatedLength);
            throw new ArgumentOutOfRangeException(nameof(packetBytes),
                $"Packet length mismatch, expected: {indicatedLength}, actual: {packetBytes.Length}");
        }

        throw new NotImplementedException();
    }
}