using System.Net;

namespace RadiusCore;

/// <summary>
/// RADIUS上下文。
/// </summary>
public class RadiusContext
{
    private RadiusPacket2 radiusPacket;
    private IPEndPoint remoteEndPoint;

    public RadiusContext(RadiusPacket2 radiusPacket, IPEndPoint remoteEndPoint)
    {
        this.radiusPacket = radiusPacket;
        this.remoteEndPoint = remoteEndPoint;
    }
}