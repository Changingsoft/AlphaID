using System.Net;
using System.Text;
using RadiusCore.Packet;
using RadiusCore.RadiusConstants;

namespace RadiusCore.Tests
{
    /// <summary>
    /// Creates a response packet according to example in https://tools.ietf.org/html/rfc2865
    /// </summary>
    public sealed class MockPacketHandler : IPacketHandler
    {
        public RadiusPacket HandlePacket(RadiusPacket packet)
        {
            if (packet.Code == PacketCode.AccessRequest)
            {
                if (packet.GetAttribute<string>("User-Password") == "arctangent")
                {
                    var responsePacket = packet.CreateResponsePacket(PacketCode.AccessAccept);
                    responsePacket.AddAttribute("Service-Type", 1);
                    responsePacket.AddAttribute("Login-Service", 0);
                    responsePacket.AddAttribute("Login-IP-Host", IPAddress.Parse("192.168.1.3"));
                    return responsePacket;
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
            throw new InvalidOperationException("Couldn't handle request?!");
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }
    }
}