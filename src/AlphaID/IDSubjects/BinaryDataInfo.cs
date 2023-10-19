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
    /// MIME类型
    /// </summary>
    [MaxLength(100), Unicode(false)]
    public string MimeType { get; set; } = default!;

    /// <summary>
    /// 数据。
    /// </summary>
    public byte[] Data { get; set; } = default!;
}
