using RadiusCore.Packet;
using RadiusCore.RadiusConstants;
using System.Net;

namespace RadiusCore;

/// <summary>
/// 表示一个RADIUS报文。
/// </summary>
public class RadiusRequest(RadiusPacketDataStruct radiusPacketStruct, List<RadiusAttributeStruct> attributes, IPEndPoint remote)
{

    /// <summary>
    /// RADIUS Packet code.
    /// </summary>
    public PacketCode Code => (PacketCode)radiusPacketStruct.Code;

    /// <summary>
    /// RADIUS Packet identifier.
    /// </summary>
    public byte Identifier => radiusPacketStruct.Identifier;

    /// <summary>
    /// Authenticator.
    /// </summary>
    public byte[] Authenticator => radiusPacketStruct.Authenticator;

    /// <summary>
    /// Remote endpoint.
    /// </summary>
    public IPEndPoint Remote => remote;

    /// <summary>
    /// Attributes.
    /// </summary>
    public List<RadiusAttributeStruct> Attributes => attributes;
}