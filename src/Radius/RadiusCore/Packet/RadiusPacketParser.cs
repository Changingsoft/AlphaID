using Microsoft.Extensions.Logging;
using RadiusCore.Dictionary;
using RadiusCore.RadiusConstants;

namespace RadiusCore.Packet;

/// <summary>
/// RadiusPacketParser
/// </summary>
public class RadiusPacketParser(
    IRadiusDictionary radiusDictionary,
    ILogger<RadiusPacketParser>? logger,
    bool skipBlastRadiusChecks = true) : IRadiusPacketParser
{
    /// <summary>
    /// Parses packet bytes and returns an IRadiusPacket
    /// </summary>
    public RadiusPacket Parse(byte[] packetBytes)
    {
        var packetLength = BitConverter.ToUInt16([.. packetBytes.Skip(2).Take(2).Reverse()], 0);
        if (packetBytes.Length < packetLength)
        {
            throw new ArgumentOutOfRangeException(nameof(packetBytes),
                $"Packet length mismatch, expected: {packetLength}, actual: {packetBytes.Length}");
        }

        var packet = new RadiusPacket
        {
            Identifier = packetBytes[1],
            Code = (PacketCode)packetBytes[0],
            Authenticator = [.. packetBytes.Skip(4).Take(16)],
        };


        var messageAuthenticatorPosition = AddAttributesToPacket(packet, packetBytes, packetLength);

        // check blast radius for all Access* packets
        if (packet.Code == PacketCode.AccessAccept
            || packet.Code == PacketCode.AccessChallenge
            || packet.Code == PacketCode.AccessReject
            || packet.Code == PacketCode.AccessRequest)
        {
            if (messageAuthenticatorPosition == 0 && !skipBlastRadiusChecks)
            {
                throw new MessageAuthenticatorException("No message authenticator found in packet");
            }

            if (messageAuthenticatorPosition != 20 && !skipBlastRadiusChecks)
            {
                logger?.LogWarning("Message authenticator expected to be first attribute");
            }
        }

        return packet;
    }


    /// <summary>
    /// Populate packet with attributes and return position of Message-Authenticator if found
    /// Yees, very mutating... anyway
    /// </summary>
    /// <returns>Message-Authenticator position if found</returns>
    private int AddAttributesToPacket(RadiusPacket packet, byte[] packetBytes, int packetLength)
    {
        var position = 20;
        var messageAuthenticatorPosition = 0;

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
                    if (attributeType.Code == 80)
                    {
                        messageAuthenticatorPosition = position;
                    }

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

        return messageAuthenticatorPosition;
    }
}