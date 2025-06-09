using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Directory Service. e.g. Microsoft Active Directory.
/// </summary>
[Table("DirectoryService")]
public class DirectoryService
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Name
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 获取LDAP的类型。
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public LdapType Type { get; set; }

    /// <summary>
    /// Server (Host address and port)
    /// </summary>
    [MaxLength(50)]
    public string ServerAddress { get; set; } = null!;

    /// <summary>
    /// Root DN.
    /// </summary>
    [MaxLength(150)]
    public string RootDn { get; set; } = null!;

    /// <summary>
    /// Default User Account OU Path
    /// </summary>
    [MaxLength(150)]
    public string DefaultUserAccountContainer { get; set; } = null!;

    /// <summary>
    /// UserName.
    /// </summary>
    [MaxLength(50)]
    public string? UserName { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string? Password { get; set; }

    /// <summary>
    /// UPN suffix.
    /// </summary>
    [MaxLength(20)]
    [Unicode(false)]
    public string UpnSuffix { get; set; } = null!;

    /// <summary>
    /// SAMAccountName prefix domain name.
    /// </summary>
    [MaxLength(10)]
    [Unicode(false)]
    public string? SamDomainPart { get; set; } = null!;

    /// <summary>
    /// 获取或设置外部登录提供器信息。
    /// </summary>
    public virtual ExternalLoginProviderInfo? ExternalLoginProvider { get; set; }

    /// <summary>
    /// 自动创建账户。
    /// </summary>
    public bool AutoCreateAccount { get; set; } = false;

    
}