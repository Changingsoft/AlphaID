using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects.WechatWebLogin;

/// <summary>
/// 微信公众号。
/// </summary>
[Table("WechatService")]
public class WechatService
{
    /// <summary>
    /// AppId.
    /// </summary>
    [Key]
    [MaxLength(50), Unicode(false)]
    public string AppId { get; set; } = default!;

    /// <summary>
    /// Secret.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string Secret { get; set; } = default!;
}
