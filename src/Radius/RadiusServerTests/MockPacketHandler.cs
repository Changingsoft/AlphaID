using Flexinets.Radius.Core;
using System;
using System.Net;
using System.Text;

namespace Flexinets.Radius
{
    /// <summary>
    /// Creates a response packet according to example in https://tools.ietf.org/html/rfc2865
    /// </summary>
    public sealed class MockPacketHandler : IPacketHandler
    {
        public IRadiusPacket HandlePacket(IRadiusPacket packet)
        {
            if (packet.Code == PacketCode.AccessRequest)
            {
                if (packet.GetAttribute<String>("User-Password") == "arctangent")
                {
                    var responsepacket = packet.CreateResponsePacket(PacketCode.AccessAccept);
                    responsepacket.AddAttribute("Service-Type", 1);
                    responsepacket.AddAttribute("Login-Service", 0);
                    responsepacket.AddAttribute("Login-IP-Host", IPAddress.Parse("192.168.1.3"));
                    return responsepacket;
                }
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Packet dump for {packet.Identifier}:");
            foreach (var attribute in packet.Attributes)
            {
                attribute.Value.ForEach(o => sb.AppendLine($"{attribute.Key} : {o} [{o.GetType()}]"));
            }
            Console.WriteLine(sb.ToString());
            //Console.WriteLine(packet.GetAttribute<String>("3GPP-GGSN-MCC-MNC"));
            throw new InvalidOperationException("Couldnt handle request?!");
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}