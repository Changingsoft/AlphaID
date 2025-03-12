using RadiusCore.Packet;

namespace RadiusCore
{
    public interface IPacketHandler : IDisposable
    {
        RadiusPacket HandlePacket(RadiusPacket packet);
    }
}