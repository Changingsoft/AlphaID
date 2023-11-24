using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdSubjects.RealName.Requesting;
/// <summary>
/// 表示一个实名验证请求
/// </summary>
[Table("RealNameRequest")]
public abstract class RealNameRequest
{
    /// <summary>
    /// 
    /// </summary>
    [Key]
    public int Id { get; protected internal set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(50),Unicode(false)]
    public string PersonId { get; protected internal set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset WhenCommitted { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public bool? Accepted { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    [MaxLength(30)]
    public string? Auditor { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset? AcceptedAt { get; protected set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="auditor"></param>
    /// <param name="time"></param>
    public void SetAudit(bool accept, string? auditor, DateTimeOffset time)
    {
        this.Accepted = accept;
        this.Auditor = auditor;
        this.AcceptedAt = time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract RealNameAuthentication CreateAuthentication();
}
