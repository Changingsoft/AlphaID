using System.Runtime.InteropServices;

namespace RadiusCore.Packet;

/// <summary>
/// This struct represents a Radius attribute data in memory
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RadiusAttributeStruct
{
    /// <summary>
    /// The type of the attribute
    /// </summary>
    public byte Type;

    /// <summary>
    /// The length of the attribute
    /// </summary>
    public byte Length;

    /// <summary>
    /// The value of the attribute
    /// </summary>
    public IntPtr Value;

    public byte[] GetValue()
    {
        byte[] valueBytes = new byte[Length - 2];
        Marshal.Copy(Value, valueBytes, 0, valueBytes.Length);
        return valueBytes;
    }
}