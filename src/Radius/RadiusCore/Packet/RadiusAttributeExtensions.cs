using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RadiusCore.Packet
{
    public static class RadiusAttributeExtensions
    {
        public static byte[] ToByteArray(this RadiusAttribute attribute)
        {
            byte[] valueBytes = new byte[attribute.Length - 2];
            Marshal.Copy(attribute.Value, valueBytes, 0, valueBytes.Length);

            byte[] result = new byte[attribute.Length];
            result[0] = attribute.Type;
            result[1] = attribute.Length;
            Array.Copy(valueBytes, 0, result, 2, valueBytes.Length);

            return result;
        }

        public static RadiusAttribute FromByteArray(byte[] data)
        {
            if (data.Length < 2)
                throw new ArgumentException("Invalid attribute data");

            RadiusAttribute attribute = new RadiusAttribute
            {
                Type = data[0],
                Length = data[1],
                Value = Marshal.AllocHGlobal(data.Length - 2)
            };

            Marshal.Copy(data, 2, attribute.Value, data.Length - 2);

            return attribute;
        }

        public static void Free(this RadiusAttribute attribute)
        {
            if (attribute.Value != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(attribute.Value);
                attribute.Value = IntPtr.Zero;
            }
        }
    }
}
