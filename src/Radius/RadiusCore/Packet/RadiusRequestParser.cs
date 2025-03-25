using Microsoft.Extensions.Logging;
using RadiusCore.Dictionary;

namespace RadiusCore.Packet;

/// <summary>
/// RadiusRequestParser
/// </summary>
public class RadiusRequestParser(
    IRadiusDictionary radiusDictionary,
    ILogger<RadiusRequestParser>? logger)
{
    /// <summary>
    /// Parses packet bytes and returns an IRadiusPacket
    /// </summary>
    public RadiusRequest Parse(byte[] packetBytes)
    {
        var packetLength = BitConverter.ToUInt16([.. packetBytes.Skip(2).Take(2).Reverse()], 0);
        if (packetBytes.Length < packetLength)
        {
            logger?.LogError("收到的数据包长度小于预期。数据包长度是{ByteArrayLength}，预期长度是{PacketLength}。", packetBytes.Length, packetLength);
            throw new ArgumentOutOfRangeException(nameof(packetBytes),
                $"Packet length mismatch, expected: {packetLength}, actual: {packetBytes.Length}");
        }

        throw new NotImplementedException();
    }


    /// <summary>
    /// Populate packet with attributes and return position of Message-Authenticator if found
    /// Yees, very mutating... anyway
    /// </summary>
    /// <returns>Message-Authenticator position if found</returns>
    private void AddAttributesToPacket(RadiusPacket packet, byte[] packetBytes, int packetLength)
    {
        var position = 20;

        while (position < packetLength)
        {
            var typeCode = packetBytes[position];
            var attributeLength = packetBytes[position + 1];
            var attributeValueBytes = packetBytes.Skip(position + 2).Take(attributeLength - 2).ToArray();

            try
            {
                if (typeCode == 26) // VSA
                {
                    var vsa = new VendorSpecificAttribute(attributeValueBytes);
                    var vsaType = radiusDictionary.GetVendorAttribute(vsa.VendorId, vsa.VendorCode);

                    if (vsaType == null)
                    {
                        logger?.LogInformation("Unknown vsa: {id}:{code}", vsa.VendorId, vsa.VendorCode);
                    }
                    else
                    {
                        try
                        {
                            packet.AddAttributeObject(
                                vsaType.Name,
                                Attribute.ToObject(
                                    vsa.Value,
                                    vsaType.Type,
                                    packet.Authenticator));
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Something went wrong with vsa {name}", vsaType.Name);
                        }
                    }
                }
                else
                {
                    var attributeType = radiusDictionary.GetAttribute(typeCode) ??
                                        throw new ArgumentNullException(nameof(typeCode));

                    try
                    {
                        packet.AddAttributeObject(
                            attributeType.Name,
                            Attribute.ToObject(
                                attributeValueBytes,
                                attributeType.Type,
                                packet.Authenticator));
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex, "Something went wrong with {attributeTypeName}", attributeType.Name);
                        logger?.LogDebug("Attribute bytes: {hex}", attributeValueBytes.ToHexString());
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                logger?.LogWarning("Attribute {typeCode} not found in dictionary", typeCode);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Something went wrong parsing attribute {typeCode}", typeCode);
            }

            position += attributeLength;
        }
    }
}