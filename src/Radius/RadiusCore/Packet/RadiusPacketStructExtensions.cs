using System.Runtime.InteropServices;

namespace RadiusCore.Packet;

/// <summary>
/// Extension methods for RadiusPacketStruct
/// </summary>
public static class RadiusPacketStructExtensions
{
    /// <summary>
    /// Convert a RadiusPacketStruct and a list of RadiusAttributes to a byte array
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this RadiusPacketStruct packet, List<RadiusAttribute> attributes)
    {
        int size = Marshal.SizeOf(packet) + attributes.Count * Marshal.SizeOf(typeof(RadiusAttribute));
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, arr, 0, Marshal.SizeOf(packet));

            int offset = Marshal.SizeOf(packet);
            foreach (var attribute in attributes)
            {
                Marshal.StructureToPtr(attribute, ptr + offset, true);
                Marshal.Copy(ptr + offset, arr, offset, Marshal.SizeOf(attribute));
                offset += Marshal.SizeOf(attribute);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return arr;
    }

    /// <summary>
    /// Convert a byte array to a RadiusPacketStruct and a list of RadiusAttributes
    /// </summary>
    /// <param name="data"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static RadiusPacketStruct FromByteArray(byte[] data, out List<RadiusAttribute> attributes)
    {
        RadiusPacketStruct packet = new();
        int size = Marshal.SizeOf(packet);
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.Copy(data, 0, ptr, size);
            packet = (RadiusPacketStruct)Marshal.PtrToStructure(ptr, packet.GetType())!;

            attributes = [];
            int offset = size;
            while (offset < data.Length)
            {
                RadiusAttribute attribute = new();
                int attrSize = Marshal.SizeOf(attribute);
                IntPtr attrPtr = Marshal.AllocHGlobal(attrSize);

                try
                {
                    Marshal.Copy(data, offset, attrPtr, attrSize);
                    attribute = (RadiusAttribute)Marshal.PtrToStructure(attrPtr, attribute.GetType())!;
                    attributes.Add(attribute);
                    offset += attribute.Length;
                }
                finally
                {
                    Marshal.FreeHGlobal(attrPtr);
                }
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return packet;
    }
}