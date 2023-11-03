using IDSubjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace AlphaID.EntityFramework;

[Table("UserClaim")]
public class NaturalPersonClaim
{
    /// <summary>
    /// Gets or sets the identifier for this user claim.
    /// </summary>
    [Key]
    public virtual int Id { get; set; } = default!;

    /// <summary>
    /// Gets or sets the primary key of the user associated with this claim.
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public virtual string UserId { get; set; } = default!;

    /// <summary>
    /// Gets or sets the claim type for this claim.
    /// </summary>
    [MaxLength(256), Unicode(false)]
    public virtual string? ClaimType { get; set; }

    /// <summary>
    /// Gets or sets the claim value for this claim.
    /// </summary>
    [MaxLength(50)]
    public virtual string? ClaimValue { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual NaturalPerson User { get; set; } = default!;

    /// <summary>
    /// Converts the entity into a Claim instance.
    /// </summary>
    /// <returns></returns>
    public virtual Claim ToClaim()
    {
        return new Claim(ClaimType!, ClaimValue!);
    }

    /// <summary>
    /// Reads the type and value from the Claim.
    /// </summary>
    /// <param name="claim"></param>
    public virtual void InitializeFromClaim(Claim claim)
    {
        ClaimType = claim.Type;
        ClaimValue = claim.Value;
    }
}
