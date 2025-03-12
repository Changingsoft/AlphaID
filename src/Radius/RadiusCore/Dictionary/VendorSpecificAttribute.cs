namespace RadiusCore.Dictionary
{
    /// <summary>
    /// Create a vsa from bytes
    /// </summary>
    /// <param name="contentBytes"></param>
    public class VendorSpecificAttribute(byte[] contentBytes)
    {
        public readonly byte Length = contentBytes[5];
        public readonly uint VendorId = BitConverter.ToUInt32([.. contentBytes.Take(4).Reverse()], 0);
        public readonly byte VendorCode = contentBytes[4];
        public readonly byte[] Value = [.. contentBytes.Skip(6)];
    }
}