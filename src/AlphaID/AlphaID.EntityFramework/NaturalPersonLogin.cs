using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IdSubjects;
using Microsoft.EntityFrameworkCore;

namespace AlphaId.EntityFramework;

[Table("NaturalPersonLogin")]
[Index(nameof(UserId))]
[PrimaryKey(nameof(LoginProvider), nameof(ProviderKey))]
public class NaturalPersonLogin
{
    /// <summary>
    ///     Gets or sets the login provider for the login (e.g. facebook, google)
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public virtual string LoginProvider { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the unique provider identifier for this login.
    /// </summary>
    [MaxLength(128)]
    [Unicode(false)]
    public virtual string ProviderKey { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the friendly name used in a UI for this login.
    /// </summary>
    [MaxLength(50)]
    public virtual string? ProviderDisplayName { get; set; }

    /// <summary>
    ///     Gets or sets the primary key of the user associated with this login.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public virtual string UserId { get; set; } = default!;

    [ForeignKey(nameof(UserId))]
    public virtual NaturalPerson User { get; set; } = default!;
}