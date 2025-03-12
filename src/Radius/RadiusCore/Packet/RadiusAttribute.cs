using System.Runtime.InteropServices;

namespace RadiusCore.Packet
{
    /// <summary>
    /// This struct represents a Radius attribute in memory
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RadiusAttribute
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 253)]
        public byte[] Value;
    }
}
