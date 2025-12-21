namespace Organizational;

/// <summary>
/// 组织的人员。
/// </summary>
public class OrganizationMember
{
    /// <summary>
    /// </summary>
    protected OrganizationMember()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="visibility"></param>
    /// 
    /// <exception cref="ArgumentNullException"></exception>
    public OrganizationMember(string userId, MembershipVisibility visibility)
    {
        PersonId = userId;
        Visibility = visibility;
    }

    /// <summary>
    /// Person Id.
    /// </summary>
    public string PersonId { get; protected set; } = null!;

    /// <summary>
    /// Gets the unique identifier for the organization.
    /// </summary>
    public string OrganizationId { get; protected set; } = null!;

    /// <summary>
    /// 部门。
    /// </summary>
    public string? Department { get; set; } = null!;

    /// <summary>
    /// 职务。
    /// </summary>
    public string? Title { get; set; } = null!;

    /// <summary>
    /// 备注。
    /// </summary>
    public string? Remark { get; set; } = null!;

    /// <summary>
    /// Is Owner of the organization.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Membership visibility.
    /// </summary>
    public MembershipVisibility Visibility { get; set; } = MembershipVisibility.Private;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{PersonId}|{(IsOwner ? "Owner" : "")}|{Visibility}";
    }
}