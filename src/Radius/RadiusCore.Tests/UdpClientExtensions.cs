using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore.Tests
{
    static class UdpClientExtensions
    {
        public static MockUdpClient GetMockUdpClient(this IUdpClient udpClient)
        {
            if(udpClient is not MockUdpClient mock)
                throw new InvalidOperationException("The IUdpClient is not a MockUdpClient");
            return mock;
        }
    }
}
