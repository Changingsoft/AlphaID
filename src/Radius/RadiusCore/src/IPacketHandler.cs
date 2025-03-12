using RadiusCore.Packet;

namespace RadiusCore
{
    public interface IPacketHandler : IDisposable
    {
        IRadiusPacket HandlePacket(IRadiusPacket packet);
    }
}