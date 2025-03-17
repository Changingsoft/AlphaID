using RadiusCore.Packet;
using RadiusCore.RadiusConstants;

namespace RadiusCore;

/// <summary>
/// 表示一个RADIUS报文。
/// </summary>
public class RadiusPacket2(RadiusPacketDataStruct radiusPacketStruct, List<RadiusAttributeStruct> attributes)
{

    /// <summary>
    /// RADIUS Packet code.
    /// </summary>
    public PacketCode Code => (PacketCode)radiusPacketStruct.Code;

    /// <summary>
    /// RADIUS Packet identifier.
    /// </summary>
    public byte Identifier => radiusPacketStruct.Identifier;
}