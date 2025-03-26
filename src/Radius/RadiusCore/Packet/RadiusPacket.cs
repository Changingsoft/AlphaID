using System.Net;
using System.Security.Cryptography;

namespace RadiusCore.Packet;

/// <summary>
/// This class encapsulates a Radius packet and presents it in a more readable form
/// </summary>
public class RadiusPacket
{
    private readonly byte[] _data = new byte[20];


    /// <summary>
    /// Create a new packet with a random authenticator
    /// </summary>
    public RadiusPacket(PacketCode code, byte identifier)
    {
        _data[0] = (byte)code;
        _data[1] = identifier;

        // Generate random authenticator for access request packets
        if (Code == PacketCode.AccessRequest || Code == PacketCode.StatusServer)
        {
            using var csp = RandomNumberGenerator.Create();
            csp.GetNonZeroBytes(Authenticator);
        }

        // A Message authenticator is required in status server packets, calculated last
        if (Code == PacketCode.StatusServer)
        {
            AddMessageAuthenticator();
        }
    }
    private IList<AttributeItem> _attributes;

    internal RadiusPacket(byte[] data, IList<AttributeItem> attributeItems)
    {
        if (data.Length != 20)
            throw new ArgumentException("data must be 20 bytes long.", nameof(data));
        _data = data;
        _attributes = attributeItems;
        //todo: convert to...
    }

    /// <summary>
    /// 
    /// </summary>
    public PacketCode Code
    {
        get => (PacketCode)_data[0];
        set => _data[0] = (byte)value;
    }

    /// <summary>
    /// 
    /// </summary>
    public byte Identifier
    {
        get => _data[1];
        set => _data[1] = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public byte[] Authenticator
    {
        get => _data[4..20];
        set
        {
            // Copy the authenticator to the packet data
            if (value.Length != 16)
                throw new ArgumentOutOfRangeException(nameof(value), "Authenticator must be 16 bytes long");
            value.CopyTo(_data, 4);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public IDictionary<string, List<object>> Attributes { get; set; } = new Dictionary<string, List<object>>();

    /// <summary>
    /// Creates a response packet with code, authenticator, identifier and secret from the request packet.
    /// </summary>
    public RadiusPacket CreateResponsePacket(PacketCode responseCode) =>
        new(responseCode, Identifier);


    /// <summary>
    /// Gets a single attribute value with name cast to type
    /// Throws an exception if multiple attributes with the same name are found
    /// </summary>
    public T? GetAttribute<T>(string name) => GetAttributes<T>(name).SingleOrDefault();


    /// <summary>
    /// Gets multiple attribute values with the same name cast to type
    /// </summary>
    public List<T> GetAttributes<T>(string name) =>
        Attributes.TryGetValue(name, out var attribute)
            ? [.. attribute.Cast<T>()]
            : [];


    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddAttribute(string name, string value) => AddAttributeObject(name, value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddAttribute(string name, uint value) => AddAttributeObject(name, value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddAttribute(string name, IPAddress value) => AddAttributeObject(name, value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void AddAttribute(string name, byte[] value) => AddAttributeObject(name, value);

    /// <summary>
    /// Add a Message-Authenticator placeholder attribute to the packet
    /// The actual value is calculated when assembling the packet
    /// </summary>
    public void AddMessageAuthenticator() => AddAttribute("Message-Authenticator", new byte[16]);


    internal void AddAttributeObject(string name, object value)
    {
        if (!Attributes.ContainsKey(name))
        {
            Attributes.Add(name, []);
        }

        Attributes[name].Add(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static RadiusPacket FromByteArray(ReadOnlySpan<byte> data)
    {
        if(data.Length < 20)
            throw new ArgumentOutOfRangeException(nameof(data), "Packet data must be at least 20 bytes long");
        if(data.Length > 4096)
            throw new ArgumentOutOfRangeException(nameof(data), "Packet data is too big.");

        var indicatedLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data[2..4]));
        if(data.Length < indicatedLength)
            throw new ArgumentOutOfRangeException(nameof(data), "Packet data length does not match indicated length");

        //parse attributes
        List<AttributeItem> attributeItems = new();
        var attributesData = data[20..];
        var pos = 0;
        while (pos < attributesData.Length)
        {
            var length = attributesData[pos + 1];
            AttributeItem item = new(attributesData[pos], attributesData.Slice(pos + 2, length - 2).ToArray());
            attributeItems.Add(item);
            pos += length;
        }

        RadiusPacket radiusPacket = new RadiusPacket(data[0..20].ToArray(), attributeItems);
        return radiusPacket;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"Code: {Code}, Identifier: {Identifier}, Attributes: {Attributes.Count}";
    }
}