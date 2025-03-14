using RadiusCore.Packet;

namespace RadiusCore;

/// <summary>
/// 
/// </summary>
public interface IPacketHandler : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    RadiusPacket HandlePacket(RadiusPacket packet);
}