using RadiusCore.Packet;
using System.Net;

namespace RadiusCore;

/// <summary>
/// 表示一个RADIUS报文。
/// </summary>
public class RadiusRequest(RadiusPacket packet, IPEndPoint remote)
{
    /// <summary>
    /// Radius packet.
    /// </summary>
    public RadiusPacket Packet => packet;

    /// <summary>
    /// Remote endpoint.
    /// </summary>
    public IPEndPoint Remote => remote;

}