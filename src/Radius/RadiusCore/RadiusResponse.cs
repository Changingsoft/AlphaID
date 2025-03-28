
using System.Net;
using RadiusCore.Packet;

namespace RadiusCore;

public class RadiusResponse
{
    public RadiusPacket Packet { get; }

    public IPEndPoint? Remote { get; internal set; }

    internal byte[] ToBytes()
    {
        return Packet.ToByteArray();
    }
}