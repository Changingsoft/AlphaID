using RadiusCore.Packet;
using RadiusCore.RadiusConstants;

namespace RadiusCore;
public class RadiusPacket2
{
    private RadiusPacketDataStruct radiusPacketStruct;
    private List<RadiusAttributeStruct> attributes;

    public RadiusPacket2(RadiusPacketDataStruct radiusPacketStruct, List<RadiusAttributeStruct> attributes)
    {
        this.radiusPacketStruct = radiusPacketStruct;
        this.attributes = attributes;
    }

    public PacketCode PacketCode => (PacketCode)radiusPacketStruct.Code;

    public byte Identifier => radiusPacketStruct.Identifier;
}