using RadiusCore;
using System.Net;

namespace Radius.TestServer.PacketHandler;

public interface IPacketHandlerRepository
{
    bool TryGetHandler(IPAddress remoteAddress, out (IPacketHandler packetHandler, string sharedSecret) handler);
}