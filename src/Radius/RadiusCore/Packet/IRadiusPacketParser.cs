namespace RadiusCore.Packet;

/// <summary>
/// 
/// </summary>
public interface IRadiusPacketParser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    byte[] GetBytes(RadiusPacket packet);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="packetBytes"></param>
    /// <param name="sharedSecret"></param>
    /// <param name="requestAuthenticator"></param>
    /// <returns></returns>
    RadiusPacket Parse(byte[] packetBytes, byte[] sharedSecret, byte[]? requestAuthenticator = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="packet"></param>
    /// <param name="sharedSecret"></param>
    /// <param name="requestAuthenticator"></param>
    /// <returns></returns>
    [Obsolete("Use parse instead... this isn't async anyway")]
    bool TryParsePacketFromStream(Stream stream, out RadiusPacket? packet, byte[] sharedSecret, byte[]? requestAuthenticator = null);
}