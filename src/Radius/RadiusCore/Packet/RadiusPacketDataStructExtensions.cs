using System.Net;
using System.Runtime.InteropServices;

namespace RadiusCore.Packet;

/// <summary>
/// Extension methods for RadiusPacketDataStruct
/// </summary>
public static class RadiusPacketDataStructExtensions
{
    /// <summary>
    /// Convert a RadiusPacketDataStruct and a list of RadiusAttributes to a byte array
    /// </summary>
    /// <param name="packet"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this RadiusPacketDataStruct packet, List<RadiusAttributeStruct> attributes)
    {
        // Calculate the total length of the packet
        packet.Length = (ushort)(Marshal.SizeOf(packet) + attributes.Sum(attr => attr.Length));

        // Convert Length to network byte order (big-endian)
        packet.Length = (ushort)IPAddress.HostToNetworkOrder((short)packet.Length);

        int size = Marshal.SizeOf(packet) + attributes.Count * attributes.Sum(attr => attr.Length);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, arr, 0, Marshal.SizeOf(packet));

            int offset = Marshal.SizeOf(packet);
            foreach (var attribute in attributes)
            {
                byte[] attrBytes = attribute.ToByteArray();
                Array.Copy(attrBytes, 0, arr, offset, attrBytes.Length);
                offset += attrBytes.Length;
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return arr;
    }

    /// <summary>
    /// Convert a byte array to a RadiusPacketDataStruct and a list of RadiusAttributes
    /// </summary>
    /// <param name="data"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static RadiusPacketDataStruct FromByteArray(byte[] data, out List<RadiusAttributeStruct> attributes)
    {
        RadiusPacketDataStruct packet = new();
        int size = Marshal.SizeOf(packet);
        IntPtr ptr = Marshal.AllocHGlobal(size);

        try
        {
            Marshal.Copy(data, 0, ptr, size);
            packet = (RadiusPacketDataStruct)Marshal.PtrToStructure(ptr, packet.GetType())!;

            // Convert Length from network byte order (big-endian) to host byte order
            packet.Length = (ushort)IPAddress.NetworkToHostOrder((short)packet.Length);

            attributes = [];
            int offset = size;
            while (offset < data.Length)
            {
                byte attrType = data[offset];
                byte attrLength = data[offset + 1];
                byte[] attrData = new byte[attrLength];
                Array.Copy(data, offset, attrData, 0, attrLength);

                RadiusAttributeStruct attribute = RadiusAttributeStructExtensions.FromByteArray(attrData);
                attributes.Add(attribute);
                offset += attrLength;
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return packet;
    }

    /// <summary>
    /// Convert a byte array to a IntPtr.
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>
    public static IntPtr ToPtr(this byte[] byteArray)
    {
        IntPtr ptr = Marshal.AllocHGlobal(byteArray.Length);
        Marshal.Copy(byteArray, 0, ptr, byteArray.Length);
        return ptr;
    }
}