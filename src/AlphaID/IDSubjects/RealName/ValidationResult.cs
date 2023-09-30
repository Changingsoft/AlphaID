using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IDSubjects.RealName;

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
    /// <param name="validator"></param>
    /// <param name="validateTime"></param>
    /// <param name="accepted"></param>
    public ValidationResult(string validator, DateTime validateTime, bool accepted)
    {
        this.Validator = validator;
        this.ValidateTime = validateTime;
        this.Accepted = accepted;
    }

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
