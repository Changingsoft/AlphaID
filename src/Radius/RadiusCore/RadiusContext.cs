using System.Net;

namespace RadiusCore;

/// <summary>
/// RADIUS上下文。
/// </summary>
/// <remarks>
/// 
/// </remarks>
/// <param name="radiusPacket"></param>
/// <param name="server"></param>
public class RadiusContext(RadiusRequest radiusPacket, RadiusServer server)
{
    /// <summary>
    /// 
    /// </summary>
    public RadiusRequest Request { get; } = radiusPacket;

    /// <summary>
    /// 
    /// </summary>
    public RadiusServer Server { get; } = server;

    /// <summary>
    /// 
    /// </summary>
    public RadiusResponse Response {get;} = new();
}