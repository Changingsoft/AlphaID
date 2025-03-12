using RadiusCore;
using System.Net;

namespace RadiusServer
{
    public interface IPacketHandlerRepository
    {
        [Obsolete]
        void Add(IPNetwork remoteAddressRange, IPacketHandler packetHandler, string sharedSecret);
        [Obsolete]
        void AddPacketHandler(IPAddress remoteAddress, IPacketHandler packetHandler, string sharedSecret);
        [Obsolete]
        void AddPacketHandler(List<IPAddress> remoteAddresses, IPacketHandler packetHandler, string sharedSecret);

        bool TryGetHandler(IPAddress remoteAddress, out (IPacketHandler packetHandler, string sharedSecret) handler);
    }
}