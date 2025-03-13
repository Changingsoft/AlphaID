using System.Runtime.InteropServices;

namespace RadiusCore.Packet;

/// <summary>
/// This struct represents a Radius packet in memory
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RadiusPacketStruct
{
    /// <summary>
    /// The code of the packet
    /// </summary>
    public byte Code;
    /// <summary>
    /// The identifier of the packet
    /// </summary>
    public byte Identifier;
    /// <summary>
    /// The length of the packet
    /// </summary>
    public ushort Length;

    /// <summary>
    /// The authenticator of the packet
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] Authenticator;

    // Attributes will be handled separately as they are of variable length
}