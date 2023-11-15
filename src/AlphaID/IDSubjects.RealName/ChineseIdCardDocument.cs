using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
public class ChineseIdCardDocument : IdentityDocument
{
    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [Column(TypeName = "varchar(7)")]
    public Sex Sex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50)]
    public string Ethnicity { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(100)]
    public string Address { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(18), Unicode(false)]
    public string CardNumber { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50)]
    public string Issuer { get; set; } = default!;

    /// <summary>
    /// 
    /// </summary>
    public DateOnly IssueDate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateOnly? Expires { get; set; }
}
