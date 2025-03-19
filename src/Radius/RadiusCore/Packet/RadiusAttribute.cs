namespace RadiusCore.Packet;

/// <summary>
/// 表示一个RADIUS Attribute。
/// </summary>
public abstract class RadiusAttribute
{
    public byte Type { get; set; }

    public byte Length { get; set; }

    public byte[] Value { get; set; }
}