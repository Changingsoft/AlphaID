namespace Organizational;

/// <summary>
/// 表示用户的成员身份。
/// </summary>
public class UserMembership
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public string UserId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier for the organization.
    /// </summary>
    public string OrganizationId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the organization.
    /// </summary>
    public string OrganizationName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the title associated with the current object.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the name of the department associated with the entity.
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Gets or sets an optional remark or comment associated with the object.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the current user is the owner.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// 可见性。
    /// </summary>
    public MembershipVisibility Visibility { get; set; }
}
