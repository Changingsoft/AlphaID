using System.Net;

namespace RadiusCore
{
    public class PacketHandlerRepository : IPacketHandlerRepository
    {
        private readonly Dictionary<IPAddress, (IPacketHandler packetHandler, string secret)> _packetHandlerAddresses = [];
        private readonly Dictionary<IPNetwork, (IPacketHandler packetHandler, string secret)> _packetHandlerNetworks = [];

        /// <summary>
        /// Add packet handler for remote endpoint
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="packetHandler"></param>
        /// <param name="sharedSecret"></param>
        public void AddPacketHandler(IPAddress remoteAddress, IPacketHandler packetHandler, string sharedSecret)
        {
            _packetHandlerAddresses.Add(remoteAddress, (packetHandler, sharedSecret));
        }


        /// <summary>
        /// Add packet handler for multiple remote endpoints
        /// </summary>
        /// <param name="remoteAddresses"></param>
        /// <param name="packetHandler"></param>
        /// <param name="sharedSecret"></param>
        public void AddPacketHandler(List<IPAddress> remoteAddresses, IPacketHandler packetHandler, string sharedSecret)
        {
            foreach (var remoteAddress in remoteAddresses)
            {
                _packetHandlerAddresses.Add(remoteAddress, (packetHandler, sharedSecret));
            }
        }


        /// <summary>
        /// Add packet handler for IP range
        /// </summary>
        /// <param name="sharedSecret"></param>
        /// <param name="remoteAddressRange"></param>
        /// <param name="packetHandler"></param>
        public void Add(IPNetwork remoteAddressRange, IPacketHandler packetHandler, string sharedSecret)
        {
            _packetHandlerNetworks.Add(remoteAddressRange, (packetHandler, sharedSecret));
        }


        /// <summary>
        /// Try to find a packet handler for remote address
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool TryGetHandler(IPAddress remoteAddress, out (IPacketHandler packetHandler, string sharedSecret) handler)
        {
            if (_packetHandlerAddresses.TryGetValue(remoteAddress, out handler))
            {
                return true;
            }
            else if (_packetHandlerNetworks.Any(o => o.Key.Contains(remoteAddress)))
            {
                handler = _packetHandlerNetworks.FirstOrDefault(o => o.Key.Contains(remoteAddress)).Value;
                return true;
            }
            else if (_packetHandlerAddresses.TryGetValue(IPAddress.Any, out handler))
            {
                return true;
            }

            return false;
        }
    }
}
