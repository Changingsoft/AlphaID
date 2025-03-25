using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore.Packet;
public class AttributeParser
{
    public Dictionary<int, RadiusAttributeValue> Parse(ReadOnlySpan<byte> data)
    {
        var attributes = new Dictionary<int, RadiusAttributeValue>();
        var position = 0;
        while (position < data.Length)
        {
            var typeCode = data[position];
            var attributeLength = data[position + 1];
            var attributeValueBytes = data.Slice(position + 2, attributeLength - 2).ToArray();
            if (attributes.ContainsKey(typeCode))
            {
                attributes[typeCode].Add(attributeValueBytes);
            }
            else
            {
                attributes[typeCode] = new RadiusAttributeValue(attributeValueBytes);
            }
            position += attributeLength;
        }
        return attributes;
    }
}
