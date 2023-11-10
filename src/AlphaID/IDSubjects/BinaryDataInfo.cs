using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IDSubjects;

/// <summary>
/// 表示存储的二进制数据信息。
/// </summary>
[Owned]
public class BinaryDataInfo
{
    /// <summary>
    /// 
    /// </summary>
    protected BinaryDataInfo() { }

    /// <summary>
    /// 使用MIME类型和数据初始化二进制数据信息。
    /// </summary>
    /// <param name="mimeType"></param>
    /// <param name="data"></param>
    public BinaryDataInfo(string mimeType, byte[] data)
    {
        this.MimeType = mimeType;
        this.Data = data;
    }

    /// <summary>
    /// MIME类型
    /// </summary>
    [MaxLength(100), Unicode(false)]
    public string MimeType { get; set; } = default!;

    /// <summary>
    /// 数据。
    /// </summary>
    public byte[] Data { get; set; } = default!;

    /// <summary>
    /// 此数据的更新时间。
    /// </summary>
    public DateTimeOffset UpdateTime { get; set; } = DateTimeOffset.UtcNow;
}
