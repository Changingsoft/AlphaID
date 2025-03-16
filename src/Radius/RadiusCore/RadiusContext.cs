using System.Net;

namespace RadiusCore;
internal class RadiusContext
{
    private RadiusPacket2 radiusPacket;
    private IPEndPoint remoteEndPoint;

    public RadiusContext(RadiusPacket2 radiusPacket, IPEndPoint remoteEndPoint)
    {
        this.radiusPacket = radiusPacket;
        this.remoteEndPoint = remoteEndPoint;
    }
}