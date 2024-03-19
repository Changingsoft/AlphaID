using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdSubjects.DirectoryLogon;
/// <summary>
/// 
/// </summary>
[Owned]
public record ExternalLoginProviderInfo
{
    /// <summary>
    /// 
    /// </summary>
    protected ExternalLoginProviderInfo() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="registeredClientId"></param>
    public ExternalLoginProviderInfo(string name, string registeredClientId)
    {
        this.Name = name;
        this.RegisteredClientId = registeredClientId;
    }

    /// <summary>
    /// 名称。
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 显示名称。
    /// </summary>
    [MaxLength(50)]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Alpha ID 作为 Client 在此外部登录提供者注册的 Client Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string RegisteredClientId { get; set; } = default!;

    /// <summary>
    /// 指定Provider Key的生成器。
    /// </summary>
    [MaxLength(255), Unicode(false)]
    public string? SubjectGenerator { get; set; }
}
