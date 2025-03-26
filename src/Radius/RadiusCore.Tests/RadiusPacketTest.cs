using RadiusCore.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore.Tests;
public class RadiusPacketTest
{
    [Fact]
    public void ReadFromValidByteArray()
    {
        var data =
            Utils.StringToByteArray(
                "010000380f403f9473978057bd83d5cb98f4227a01066e656d6f02120dbe708d93d413ce3196e43f782a0aee0406c0a80110050600000003");
        RadiusPacket packet = RadiusPacket.FromByteArray(data);

        Assert.Equal(PacketCode.AccessRequest, packet.Code);
    }
}
