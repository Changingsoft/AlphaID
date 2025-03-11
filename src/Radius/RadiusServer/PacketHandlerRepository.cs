using Flexinets.Radius.Core;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Flexinets.Radius
{
    public class PacketHandlerRepository : IPacketHandlerRepository
    {
        private readonly Dictionary<IPAddress, (IPacketHandler packetHandler, String secret)> _packetHandlerAddresses = new Dictionary<IPAddress, (IPacketHandler, String)>();
        private readonly Dictionary<IPNetwork, (IPacketHandler packetHandler, String secret)> _packetHandlerNetworks = new Dictionary<IPNetwork, (IPacketHandler, String)>();

        /// <summary>
        /// Add packet handler for remote endpoint
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="packetHandler"></param>
        /// <param name="sharedSecret"></param>
        public void AddPacketHandler(IPAddress remoteAddress, IPacketHandler packetHandler, String sharedSecret)
        {
            _packetHandlerAddresses.Add(remoteAddress, (packetHandler, sharedSecret));
        }


        /// <summary>
        /// Add packet handler for multiple remote endpoints
        /// </summary>
        /// <param name="remoteAddresses"></param>
        /// <param name="packetHandler"></param>
        /// <param name="sharedSecret"></param>
        public void AddPacketHandler(List<IPAddress> remoteAddresses, IPacketHandler packetHandler, String sharedSecret)
        {
            foreach (var remoteAddress in remoteAddresses)
            {
                _packetHandlerAddresses.Add(remoteAddress, (packetHandler, sharedSecret));
            }
        }


        /// <summary>
        /// Add packet handler for IP range
        /// </summary>
        /// <param name="remoteAddresses"></param>
        /// <param name="sharedSecret"></param>
        /// <param name="packetHandler"></param>
        public void Add(IPNetwork remoteAddressRange, IPacketHandler packetHandler, String sharedSecret)
        {
            _packetHandlerNetworks.Add(remoteAddressRange, (packetHandler, sharedSecret));
        }


        /// <summary>
        /// Try to find a packet handler for remote address
        /// </summary>
        /// <param name="remoteAddress"></param>
        /// <param name="packetHandler"></param>
        /// <returns></returns>
        public Boolean TryGetHandler(IPAddress remoteAddress, out (IPacketHandler packetHandler, String sharedSecret) handler)
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
