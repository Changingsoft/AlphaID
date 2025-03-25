using RadiusCore.Packet;
using System.Net;

namespace RadiusCore;

/// <summary>
/// 表示一个RADIUS报文。
/// </summary>
public class RadiusRequest(PacketCode code, byte identifier, byte[] authenticator, List<RadiusAttributeStruct> attributes, IPEndPoint remote)
{

    /// <summary>
    /// RADIUS Packet code.
    /// </summary>
    public PacketCode Code => code;

    /// <summary>
    /// RADIUS Packet identifier.
    /// </summary>
    public byte Identifier => identifier;

    /// <summary>
    /// Authenticator.
    /// </summary>
    public byte[] Authenticator => authenticator;

    /// <summary>
    /// Remote endpoint.
    /// </summary>
    public IPEndPoint Remote => remote;

    /// <summary>
    /// Attributes.
    /// </summary>
    public List<RadiusAttributeStruct> Attributes => attributes;
}