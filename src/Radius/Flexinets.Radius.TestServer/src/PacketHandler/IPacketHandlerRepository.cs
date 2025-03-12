using System.Net;
using RadiusCore;

namespace Flexinets.Radius;

public interface IPacketHandlerRepository
{
    bool TryGetHandler(IPAddress remoteAddress, out (IPacketHandler packetHandler, string sharedSecret) handler);
}