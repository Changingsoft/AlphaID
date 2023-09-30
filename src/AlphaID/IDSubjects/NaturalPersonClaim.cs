using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace IDSubjects;

/// <summary>
/// 自然人的Claim。
/// </summary>
[Table("NaturalPersonClaim")]
public class NaturalPersonClaim
{
    /// <summary>
    /// 
    /// </summary>
    protected NaturalPersonClaim() { }

    /// <summary>
    /// Initlaize.
    /// </summary>
    /// <param name="person"></param>
    /// <param name="claimType"></param>
    /// <param name="claimValue"></param>
    public NaturalPersonClaim(NaturalPerson person, string claimType, string claimValue)
    {
        this.Person = person;
        this.UserId = person.Id;
        this.ClaimType = claimType;
        this.ClaimValue = claimValue;
    }

    /// <summary>
    /// Id.
    /// </summary>
    [Key]
    public int Id { get; protected set; }

    /// <summary>
    /// User Id.
    /// </summary>
    [Unicode(false)]
    [MaxLength(50)]
    public string UserId { get; protected set; } = default!;

    /// <summary>
    /// Person
    /// </summary>
    [ForeignKey("UserId")]
    public virtual NaturalPerson Person { get; protected set; } = default!;

    /// <summary>
    /// Claim Type.
    /// </summary>
    [Unicode(false)]
    [MaxLength(200)]
    public string ClaimType { get; set; } = default!;

    /// <summary>
    /// Claim Value.
    /// </summary>
    [MaxLength(200)]
    public string ClaimValue { get; set; } = default!;

    /// <summary>
    /// To Claim.
    /// </summary>
    /// <returns></returns>
    public Claim ToClaim()
    {
        return new Claim(this.ClaimType, this.ClaimValue);
    }
}
