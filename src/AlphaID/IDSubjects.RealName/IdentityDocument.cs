using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName;

/// <summary>
/// 
/// </summary>
[Table("IdentityDocument")]
public abstract class IdentityDocument
{
    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<DocumentAttachment> Attachments { get; protected set; } = new HashSet<DocumentAttachment>();

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset WhenCreated { get; set; } = DateTimeOffset.UtcNow;
}