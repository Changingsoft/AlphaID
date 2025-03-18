using RadiusCore.RadiusConstants;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace RadiusCore.Packet;

/// <summary>
/// This class encapsulates a Radius packet and presents it in a more readable form
/// </summary>
public class RadiusPacket
{
    private byte[] _data = new byte[20];

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

    /// <summary>
    /// 
    /// </summary>
    public RadiusPacket() { }

    /// <summary>
    /// 
    /// </summary>
    public PacketCode Code
    {
        get
        {
            return (PacketCode)_data[0];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public byte Identifier
    {
        get
        {
            return _data[1];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public byte[] Authenticator
    {
        get
        {
            return _data[4..20];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public IList<RadiusAttribute> Attributes { get; set; } = [];

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
}

public abstract class RadiusAttribute
{

}