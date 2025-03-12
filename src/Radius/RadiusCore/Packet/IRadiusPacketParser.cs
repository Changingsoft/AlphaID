namespace RadiusCore.Packet
{
    public interface IRadiusPacketParser
    {
        byte[] GetBytes(IRadiusPacket packet);
        IRadiusPacket Parse(byte[] packetBytes, byte[] sharedSecret, byte[]? requestAuthenticator = null);

        [Obsolete("Use parse instead... this isn't async anyway")]
        bool TryParsePacketFromStream(Stream stream, out IRadiusPacket? packet, byte[] sharedSecret, byte[]? requestAuthenticator = null);
    }
}