using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaID.EntityFramework;

[Table("NaturalPersonToken")]
public class NaturalPersonToken
{
    /// <summary>
    /// Gets or sets the primary key of the user that the token belongs to.
    /// </summary>
    [MaxLength(50),Unicode(false)]
    public virtual string UserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the LoginProvider this token is from.
    /// </summary>
    [MaxLength(50),Unicode(false)]
    public virtual string LoginProvider { get; set; } = default!;

    /// <summary>
    /// Gets or sets the name of the token.
    /// </summary>
    [MaxLength(50)]
    public virtual string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the token value.
    /// </summary>
    [ProtectedPersonalData]
    [MaxLength(256)]
    public virtual string? Value { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual NaturalPerson User { get; set; } = default!;
}
