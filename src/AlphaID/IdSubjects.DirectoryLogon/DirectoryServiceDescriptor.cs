using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Directory Service. e.g. Microsoft Active Directory.
/// </summary>
[Table("DirectoryService")]
public class DirectoryServiceDescriptor
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Name
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 获取LDAP的类型。
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public LdapType Type { get; set; }

    /// <summary>
    /// Server (Host address and port)
    /// </summary>
    [MaxLength(50)]
    public string ServerAddress { get; set; } = default!;

    /// <summary>
    /// Root DN.
    /// </summary>
    [MaxLength(150)]
    public string RootDn { get; set; } = default!;

    /// <summary>
    /// Default User Account OU Path
    /// </summary>
    [MaxLength(150)]
    public string DefaultUserAccountContainer { get; set; } = default!;

    /// <summary>
    /// UserName.
    /// </summary>
    [MaxLength(50)]
    public string? UserName { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string? Password { get; set; }

    /// <summary>
    /// UPN suffix.
    /// </summary>
    [MaxLength(20), Unicode(false)]
    public string UpnSuffix { get; set; } = default!;

    /// <summary>
    /// SAMAccountName prefix domain name.
    /// </summary>
    [MaxLength(10), Unicode(false)]
    public string? SamDomainPart { get; set; } = default!;

    /// <summary>
    /// 获取或设置外部登录提供器信息。
    /// </summary>
    public virtual ExternalLoginProviderInfo? ExternalLoginProvider { get; set; }

    /// <summary>
    /// 自动创建账户。
    /// </summary>
    public bool AutoCreateAccount { get; set; } = false;

    /// <summary>
    /// Gets a directory entry instance.
    /// </summary>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    internal DirectoryEntry GetRootEntry()
    {
        var host = new Uri($"LDAP://{this.ServerAddress}");
        var fqdn = new Uri(host, this.RootDn);
        var authenticationFlag = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
        DirectoryEntry entry = new($"LDAP://{fqdn.Authority}{fqdn.PathAndQuery}", null, null, authenticationFlag);
        if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password))
        {
            entry.Username = this.UserName;
            entry.Password = this.Password;
        }
        return entry;
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    internal PrincipalContext GetRootContext()
    {
        var contextType = this.Type switch
        {
            LdapType.ADDS => ContextType.Domain,
            LdapType.ADLDS => ContextType.ApplicationDirectory,
            _ => throw new NotSupportedException("不支持的LDAP类型。"),
        };
        var contextOption = ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing; //执行修改密码必须的标记。
        PrincipalContext ctx = new(contextType, this.ServerAddress, this.RootDn,
            contextOption, this.UserName, this.Password);
        return ctx;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:验证平台兼容性", Justification = "<挂起>")]
    internal PrincipalContext GetUserContainerContext()
    {
        var contextType = this.Type switch
        {
            LdapType.ADDS => ContextType.Domain,
            LdapType.ADLDS => ContextType.ApplicationDirectory,
            _ => throw new NotSupportedException("不支持的LDAP类型。"),
        };
        var contextOption = ContextOptions.Negotiate | ContextOptions.Signing | ContextOptions.Sealing; //执行修改密码必须的标记。
        PrincipalContext ctx = new(contextType, this.ServerAddress, this.DefaultUserAccountContainer,contextOption | ContextOptions.Sealing, this.UserName, this.Password);
        return ctx;
    }
}