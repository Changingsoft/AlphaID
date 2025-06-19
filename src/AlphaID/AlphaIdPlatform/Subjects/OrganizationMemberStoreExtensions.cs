namespace AlphaIdPlatform.Subjects;

/// <summary>
/// Extensions for OrganizationMemberStore.
/// </summary>
public static class OrganizationMemberStoreExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="members"></param>
    /// <param name="visitorId"></param>
    /// <returns></returns>
    public static IEnumerable<OrganizationMember> VisibleMembers(this ICollection<OrganizationMember> members,
        string? visitorId)
    {
        var orgMembers = members;
        var visibilityLevel = MembershipVisibility.Public;
        if (visitorId != null)
        {
            //如果已登录，降为AuthenticatedUser
            visibilityLevel = MembershipVisibility.AuthenticatedUser;
            //如果访问者是该组织成员，则降为Private
            if (orgMembers.Any(m => m.PersonId == visitorId))
                visibilityLevel = MembershipVisibility.Private; //Visitor is a member of the organization.
        }
        // 过滤出成员可见级别大于等于访问者最低可见级别的成员。
        return orgMembers.Where(m => m.Visibility >= visibilityLevel);
    }
}
