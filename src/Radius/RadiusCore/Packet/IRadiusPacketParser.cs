namespace RadiusCore.Packet;

/// <summary>
/// 
/// </summary>
public interface IRadiusPacketParser
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="packetBytes"></param>
    /// <returns></returns>
    RadiusPacket Parse(byte[] packetBytes);
}