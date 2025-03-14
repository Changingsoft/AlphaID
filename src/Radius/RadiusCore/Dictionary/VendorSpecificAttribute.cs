namespace RadiusCore.Dictionary;

/// <summary>
/// Create a vsa from bytes
/// </summary>
/// <param name="contentBytes"></param>
public class VendorSpecificAttribute(byte[] contentBytes)
{
    /// <summary>
    /// 
    /// </summary>
    public readonly byte Length = contentBytes[5];

    /// <summary>
    /// 
    /// </summary>
    public readonly uint VendorId = BitConverter.ToUInt32([.. contentBytes.Take(4).Reverse()], 0);

    /// <summary>
    /// 
    /// </summary>
    public readonly byte VendorCode = contentBytes[4];

    /// <summary>
    /// 
    /// </summary>
    public readonly byte[] Value = [.. contentBytes.Skip(6)];
}