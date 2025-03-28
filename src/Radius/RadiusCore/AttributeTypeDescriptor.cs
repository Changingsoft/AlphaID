using System.Text;

namespace RadiusCore
{
    public class AttributeTypeDescriptor
    {
        public byte Type { get; set; }

        public string Name { get; set; }

        public AttributeValueFormat Format { get; set; }

        public byte[] ValueToData(string value)
        {
            return Format switch
            {
                AttributeValueFormat.Text => Encoding.UTF8.GetBytes(value),
                _ => throw new NotImplementedException(),
            };
        }

        public string GetString(byte[] data)
        {
            return Format switch
            {
                AttributeValueFormat.Text => Encoding.UTF8.GetString(data),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
