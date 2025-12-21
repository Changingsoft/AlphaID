namespace Organizational;

/// <summary>
/// 表示存储的二进制数据信息。
/// </summary>
public record BinaryDataInfo
{
    /// <summary>
    /// </summary>
    protected BinaryDataInfo()
    {
    }

    /// <summary>
    /// 使用MIME类型和数据初始化二进制数据信息。
    /// </summary>
    /// <param name="mimeType"></param>
    /// <param name="data"></param>
    public BinaryDataInfo(string mimeType, byte[] data)
    {
        MimeType = mimeType;
        Data = data;
    }

    /// <summary>
    /// MIME类型
    /// </summary>
    public string MimeType { get; set; } = null!;

    /// <summary>
    /// 数据。
    /// </summary>
    public byte[] Data { get; set; } = null!;
}