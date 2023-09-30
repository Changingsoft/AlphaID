using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDSubjects;


/// <summary>
/// 自然人持有的令牌。
/// </summary>
[Table("NaturalPersonToken")]
[PrimaryKey(nameof(UserId), nameof(LoginProvider), nameof(Name))]
public class NaturalPersonToken
{
    /// <summary>
    /// 
    /// </summary>
    protected NaturalPersonToken() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="loginProvider"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public NaturalPersonToken(NaturalPerson user, string loginProvider, string name, string? value)
    {
        this.Person = user;
        this.UserId = user.Id;
        this.LoginProvider = loginProvider;
        this.Name = name;
        this.Value = value;
    }

    /// <summary>
    /// User Id.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string UserId { get; protected set; } = default!;

    /// <summary>
    /// Person.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual NaturalPerson Person { get; protected set; } = default!;

    /// <summary>
    /// Login Provider.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string LoginProvider { get; protected set; } = default!;

    /// <summary>
    /// Name.
    /// </summary>
    [MaxLength(50)]
    public string Name { get; protected set; } = default!;

    /// <summary>
    /// Value.
    /// </summary>
    [MaxLength(256), Unicode(false)]
    public string? Value { get; set; }
}
