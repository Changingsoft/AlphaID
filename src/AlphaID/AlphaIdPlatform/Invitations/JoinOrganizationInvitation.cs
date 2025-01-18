using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AlphaIdPlatform.Identity;
using Microsoft.EntityFrameworkCore;

namespace AlphaIdPlatform.Invitations;

/// <summary>
/// </summary>
[Table("JoinOrganizationInvitation")]
public class JoinOrganizationInvitation
{
    /// <summary>
    ///     Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     被邀请人Id.
    /// </summary>
    [MaxLength(50)]
    public string InviteeId { get; set; } = null!;

    /// <summary>
    ///     组织Id.
    /// </summary>
    [MaxLength(50)]
    [Unicode(false)]
    public string OrganizationId { get; set; } = null!;

    /// <summary>
    ///     邀请发出时间。
    /// </summary>
    public DateTimeOffset WhenCreated { get; set; }

    /// <summary>
    ///     邀请过期时间。
    /// </summary>
    public DateTimeOffset WhenExpired { get; set; }

    /// <summary>
    ///     发出邀请的人
    /// </summary>
    [MaxLength(50)]
    public string Inviter { get; set; } = null!;

    /// <summary>
    ///     期望的可见性。
    /// </summary>
    public MembershipVisibility ExpectVisibility { get; set; } = MembershipVisibility.Private;

    /// <summary>
    ///     只是是否已处理并已接受。如果没有值，表示未处理。true表示已接受，false表示已拒绝。
    /// </summary>
    public bool? Accepted { get; set; }
}