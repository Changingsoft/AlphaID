namespace RadiusCore.Packet
{
    public interface IRadiusPacketParser
    {
        byte[] GetBytes(RadiusPacket packet);
        RadiusPacket Parse(byte[] packetBytes, byte[] sharedSecret, byte[]? requestAuthenticator = null);

        [Obsolete("Use parse instead... this isn't async anyway")]
        bool TryParsePacketFromStream(Stream stream, out RadiusPacket? packet, byte[] sharedSecret, byte[]? requestAuthenticator = null);
    }
}