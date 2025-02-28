using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
    /// <param name="personId"></param>
    protected RealNameRequest(string personId)
    {
        PersonId = personId;
    }

    /// <summary>
    /// 
    /// </summary>
    protected RealNameRequest()
    {
    }

    /// <summary>
    /// Id。
    /// </summary>
    [Key]
    public int Id { get; protected internal set; }

    /// <summary>
    /// 与该请求关联的自然人 Id。
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string PersonId { get; protected internal set; } = null!;

    /// <summary>
    /// 提交时间。
    /// </summary>
    public DateTimeOffset WhenCommitted { get; protected internal set; }

    /// <summary>
    /// 审核是否通过。
    /// </summary>
    public bool? Accepted { get; protected set; }

    /// <summary>
    /// 审核人。
    /// </summary>
    [MaxLength(30)]
    public string? Auditor { get; protected set; }

    /// <summary>
    /// 审核时间。
    /// </summary>
    public DateTimeOffset? AuditTime { get; protected set; }

    /// <summary>
    /// 设置审核状态。
    /// </summary>
    /// <param name="accept"></param>
    /// <param name="auditor"></param>
    /// <param name="time"></param>
    public void SetAudit(bool accept, string? auditor, DateTimeOffset time)
    {
        Accepted = accept;
        Auditor = auditor;
        AuditTime = time;
    }

    /// <summary>
    /// 创建实名认证信息。
    /// </summary>
    /// <returns></returns>
    public abstract RealNameAuthentication CreateAuthentication();
}