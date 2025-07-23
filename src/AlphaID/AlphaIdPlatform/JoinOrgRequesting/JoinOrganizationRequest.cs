using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaIdPlatform.JoinOrgRequesting;

/// <summary>
/// 加入组织请求实体。
/// </summary>
[Table("JoinOrganizationRequest")]
[Index(nameof(WhenCreated))]
public class JoinOrganizationRequest
{
    /// <summary>
    /// 
    /// </summary>
    protected JoinOrganizationRequest()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="organizationName"></param>
    /// <param name="organizationId"></param>
    public JoinOrganizationRequest(string userId, string organizationName, string organizationId)
    {
        UserId = userId;
        OrganizationName = organizationName;
        OrganizationId = organizationId;
    }

    /// <summary>
    /// 
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    /// 获取或设置用户的唯一标识符。
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string UserId { get; set; } = null!;

    /// <summary>
    /// 获取或设置组织名称。
    /// </summary>
    [MaxLength(50)]
    public string OrganizationName { get; set; } = null!;

    /// <summary>
    /// 获取或设置组织的唯一标识符。
    /// </summary>
    [MaxLength(50), Unicode(false)]
    public string OrganizationId { get; set; } = null!;

    /// <summary>
    /// 获取或设置对象创建时的时间戳。
    /// </summary>
    public DateTimeOffset WhenCreated { get; protected set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 获取或设置审核执行时的时间戳。
    /// </summary>
    public DateTimeOffset? AuditAt { get; set; }

    /// <summary>
    /// 获取或设置负责审核该实体的用户或系统名称。
    /// </summary>
    [MaxLength(50)]
    public string? AuditBy { get; set; }

    /// <summary>
    /// 获取或设置操作是否被接受的值。
    /// </summary>
    public bool? IsAccepted { get; set; }
}
