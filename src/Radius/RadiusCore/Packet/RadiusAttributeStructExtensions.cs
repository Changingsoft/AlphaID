using System.Runtime.InteropServices;

namespace RadiusCore.Packet
{
    /// <summary>
    /// Extension methods for RadiusAttributeStruct
    /// </summary>
    public static class RadiusAttributeStructExtensions
    {
        /// <summary>
        /// Convert a RadiusAttributeStruct to a byte array
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this RadiusAttributeStruct attribute)
        {
            byte[] valueBytes = new byte[attribute.Length - 2];
            Marshal.Copy(attribute.Value, valueBytes, 0, valueBytes.Length);

            byte[] result = new byte[attribute.Length];
            result[0] = attribute.Type;
            result[1] = attribute.Length;
            Array.Copy(valueBytes, 0, result, 2, valueBytes.Length);

            return result;
        }

        /// <summary>
        /// Convert a byte array to a RadiusAttributeStruct
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static RadiusAttributeStruct FromByteArray(byte[] data)
        {
            if (data.Length < 2)
                throw new ArgumentException("Invalid attribute data");

            RadiusAttributeStruct attribute = new RadiusAttributeStruct
            {
                Type = data[0],
                Length = data[1],
                Value = Marshal.AllocHGlobal(data.Length - 2)
            };

            Marshal.Copy(data, 2, attribute.Value, data.Length - 2);

            return attribute;
        }

        /// <summary>
        /// Free the memory allocated for the value of the attribute
        /// </summary>
        /// <param name="attribute"></param>
        public static void Free(this RadiusAttributeStruct attribute)
        {
            if (attribute.Value != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(attribute.Value);
                attribute.Value = IntPtr.Zero;
            }
        }
    }
}
