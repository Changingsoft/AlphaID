using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.DirectoryServices;

namespace IDSubjects.DirectoryLogon;

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
    public string Name { get; set; } = default!;


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
    public string DefaultUserAccountOu { get; set; } = default!;

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
    public string SamDomainPart { get; set; } = default!;

    /// <summary>
    /// 外部登录提供器名称。
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string? ExternalLoginProvider { get; set; } = default!;

    /// <summary>
    /// 外部登录提供器显示名称。
    /// </summary>
    [MaxLength(50)]
    public string? ExternalLoginProviderName { get; set; } = default!;

    /// <summary>
    /// Gets a directory entry instance.
    /// </summary>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    public DirectoryEntry GetRootEntry()
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<挂起>")]
    internal DirectoryEntry GetUserOuEntry()
    {
        var host = new Uri($"LDAP://{this.ServerAddress}");
        var fqdn = new Uri(host, this.DefaultUserAccountOu);
        var authenticationFlag = AuthenticationTypes.Signing | AuthenticationTypes.Sealing | AuthenticationTypes.Secure;
        //var authenticationFlag = AuthenticationTypes.Secure;
        DirectoryEntry entry = new($"LDAP://{fqdn.Authority}{fqdn.PathAndQuery}", null, null, authenticationFlag);
        if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password))
        {
            entry.Username = this.UserName;
            entry.Password = this.Password;
        }
        return entry;
    }
}
