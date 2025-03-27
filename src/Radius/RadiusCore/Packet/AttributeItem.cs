using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore.Packet;
public class AttributeItem
{
    public byte Type { get; set; }
    public byte Length { get; set; }
    public byte[] Data { get; set; }
    public AttributeItem(byte type, byte[] data)
    {
        if (type == 0)
            throw new ArgumentException(nameof(type), "Type can not be a zero.");

        if (data.Length == 0)
            throw new ArgumentOutOfRangeException(nameof(data), "Data length must be greater than 0 bytes");
        if (data.Length > 253)
            throw new ArgumentOutOfRangeException(nameof(data), "Data length must be less than 253 bytes");
        if(data.All(b => b == 0))
            throw new ArgumentOutOfRangeException(nameof(data), "Data cannot be all zeros");

        Type = type;
        Length = (byte)(data.Length + 2);
        Data = data;
    }
    public byte[] GetBytes()
    {
        var bytes = new byte[Length];
        bytes[0] = Type;
        bytes[1] = Length;
        Array.Copy(Data, 0, bytes, 2, Data.Length);
        return bytes;
    }

    public override string ToString()
    {
        return $"Type: {Type}, Length: {Length}";
    }
}
