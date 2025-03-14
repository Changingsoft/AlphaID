using System.Net;

namespace RadiusCore;

/// <summary>
/// 
/// </summary>
public interface IPacketHandlerRepository
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="remoteAddressRange"></param>
    /// <param name="packetHandler"></param>
    /// <param name="sharedSecret"></param>
    [Obsolete]
    void Add(IPNetwork remoteAddressRange, IPacketHandler packetHandler, string sharedSecret);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="remoteAddress"></param>
    /// <param name="packetHandler"></param>
    /// <param name="sharedSecret"></param>
    [Obsolete]
    void AddPacketHandler(IPAddress remoteAddress, IPacketHandler packetHandler, string sharedSecret);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="remoteAddresses"></param>
    /// <param name="packetHandler"></param>
    /// <param name="sharedSecret"></param>
    [Obsolete]
    void AddPacketHandler(List<IPAddress> remoteAddresses, IPacketHandler packetHandler, string sharedSecret);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="remoteAddress"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    bool TryGetHandler(IPAddress remoteAddress, out (IPacketHandler packetHandler, string sharedSecret) handler);
}