
namespace IdSubjects.DirectoryLogon;

/// <summary>
/// Directory Service. e.g. Microsoft Active Directory.
/// </summary>
public class DirectoryService
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 获取LDAP的类型。
    /// </summary>
    public LdapType Type { get; set; }

    /// <summary>
    /// Server (Host address and port)
    /// </summary>
    public string ServerAddress { get; set; } = null!;

    /// <summary>
    /// Root DN.
    /// </summary>
    public string RootDn { get; set; } = null!;

    /// <summary>
    /// Default User Account OU Path
    /// </summary>
    public string DefaultUserAccountContainer { get; set; } = null!;

    /// <summary>
    /// UserName.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// UPN suffix.
    /// </summary>
    public string UpnSuffix { get; set; } = null!;

    /// <summary>
    /// SAMAccountName prefix domain name.
    /// </summary>
    public string? SamDomainPart { get; set; } = null!;

    /// <summary>
    /// 自动创建账户。
    /// </summary>
    public bool AutoCreateAccount { get; set; } = false;


}