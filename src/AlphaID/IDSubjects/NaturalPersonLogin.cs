using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects;

/// <summary>
/// 自然人的外部登录。
/// </summary>
[Table("NaturalPersonLogin")]
[PrimaryKey(nameof(LoginProvider), nameof(ProviderKey))]
public class NaturalPersonLogin
{
    /// <summary>
    /// 
    /// </summary>
    protected NaturalPersonLogin() { }

    /// <summary>
    /// ctor.
    /// </summary>
    /// <param name="loginProvider"></param>
    /// <param name="providerKey"></param>
    /// <param name="providerDisplayName"></param>
    /// <param name="person"></param>
    public NaturalPersonLogin(string loginProvider, string providerKey, string? providerDisplayName, NaturalPerson person)
    {
        this.LoginProvider = loginProvider;
        this.ProviderKey = providerKey;
        this.ProviderDisplayName = providerDisplayName;
        this.Person = person;
        this.UserId = person.Id;
    }

    /// <summary>
    /// 登录提供器名称。
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string LoginProvider { get; protected set; } = default!;

    /// <summary>
    /// 账户的登录标识。
    /// </summary>
    [MaxLength(128), Unicode(false)]
    public string ProviderKey { get; protected set; } = default!;

    /// <summary>
    /// 登录提供器的显示名称。
    /// </summary>
    [MaxLength(50)]
    public string? ProviderDisplayName { get; protected set; }

    /// <summary>
    /// Natural Person Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string UserId { get; protected set; } = default!;

    /// <summary>
    /// Person.
    /// </summary>
    [ForeignKey("UserId")]
    public virtual NaturalPerson Person { get; protected set; } = default!;
}
