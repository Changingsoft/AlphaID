using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 表示一个实名认证状态。
/// </summary>
[Table("RealNameState")]
public class RealNameState
{
    /// <summary>
    /// 
    /// </summary>
    protected RealNameState() { }

    /// <summary>
    /// Initialize new RealNameState.
    /// </summary>
    /// <param name="personId"></param>
    public RealNameState(string personId)
    {
        this.PersonId = personId;
    }

    /// <summary>
    /// 实名认证状态所指向的PersonId。
    /// </summary>
    [Key]
    [MaxLength(50), Unicode(false)]
    public string PersonId { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset AcceptedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50)]
    public string AcceptedBy { get; set; } = default!;

    /// <summary>
    /// 获取与此实名认证状态有关的实名认证资料。
    /// </summary>
    public virtual ICollection<RealNameValidation> Validations { get; protected set; } = new HashSet<RealNameValidation>();

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "varchar(15)")]
    public ActionIndicator ActionIndicator { get; set; }
}