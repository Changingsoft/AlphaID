using Flexinets.Radius.Core;
using System;

namespace Flexinets.Radius
{
    /// <summary>
    /// Demonstration of basic packet handler with a static username and password
    /// </summary>
    public sealed class TestPacketHandler : IPacketHandler
    {
        public IRadiusPacket HandlePacket(IRadiusPacket packet)
        {
            // Simulate lag
            //Thread.Sleep(new Random().Next(100, 3000));

            if (packet.Code == PacketCode.AccountingRequest)
            {
                var acctStatusType = packet.GetAttribute<AcctStatusType>("Acct-Status-Type");
                if (acctStatusType == AcctStatusType.Start)
                {
                    return packet.CreateResponsePacket(PacketCode.AccountingResponse);
                }

                if (acctStatusType == AcctStatusType.Stop)
                {
                    return packet.CreateResponsePacket(PacketCode.AccountingResponse);
                }

                if (acctStatusType == AcctStatusType.InterimUpdate)
                {
                    return packet.CreateResponsePacket(PacketCode.AccountingResponse);
                }
            }
            else if (packet.Code == PacketCode.AccessRequest)
            {
                if (packet.GetAttribute<String>("User-Name") == "user@example.com" && packet.GetAttribute<String>("User-Password") == "1234")
                {
                    var response = packet.CreateResponsePacket(PacketCode.AccessAccept);
                    response.AddAttribute("Acct-Interim-Interval", 60);
                    return response;
                }
                return packet.CreateResponsePacket(PacketCode.AccessReject);
            }

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