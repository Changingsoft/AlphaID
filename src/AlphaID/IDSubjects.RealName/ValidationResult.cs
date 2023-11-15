using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
[Owned]
public class ValidationResult
{
    /// <summary>
    /// 
    /// </summary>
    protected ValidationResult() { }


    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50)]
    public virtual string Validator { get; protected set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public virtual DateTime ValidateTime { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual bool Accepted { get; protected set; }

}
