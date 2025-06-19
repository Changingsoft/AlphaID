using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;

namespace AlphaIdPlatform.Subjects;

/// <summary>
/// Organization Member Manager.
/// </summary>
/// <remarks>
/// Init Organization Member Manager via Organization Member store.
/// </remarks>
/// <param name="store"></param>
public class OrganizationMemberManager(IOrganizationStore store)
{
    /// <summary>
    /// 获取指定用户的所有组织成员身份。
    /// </summary>
    /// <param name="personId"></param>
    /// <returns></returns>
    public IQueryable<UserMembership> GetMembersOf(string personId)
    {
        //获取目标person的所有组织身份。
        IQueryable<UserMembership>? members = from org in store.Organizations
                                              from member in org.Members
                                              where member.PersonId == personId
                                              select new UserMembership
                                              {
                                                  UserId = member.PersonId,
                                                  OrganizationId = org.Id,
                                                  OrganizationName = org.Name,
                                                  Title = member.Title,
                                                  Department = member.Department,
                                                  Remark = member.Remark,
                                                  IsOwner = member.IsOwner,
                                                  Visibility = member.Visibility
                                              };
        return members;
    }

    /// <summary>
    /// 以访问者visitor的视角检索指定用户的组织成员身份。
    /// </summary>
    /// <param name="personId">要检索组织成员身份的目标用户。</param>
    /// <param name="visitorId">访问者。如果传入null，代表匿名访问者。</param>
    /// <returns></returns>
    public IQueryable<UserMembership> GetVisibleMembersOf(string personId, string? visitorId)
    {
        //获取目标person的所有组织身份。
        IQueryable<UserMembership>? members = from org in store.Organizations
                                              from member in org.Members
                                              where member.PersonId == personId
                                              select new UserMembership
                                              {
                                                  UserId = member.PersonId,
                                                  OrganizationId = org.Id,
                                                  OrganizationName = org.Name,
                                                  Title = member.Title,
                                                  Department = member.Department,
                                                  Remark = member.Remark,
                                                  IsOwner = member.IsOwner,
                                                  Visibility = member.Visibility
                                              };
        Debug.Assert(members != null);

        if (visitorId == null)
            return members.Where(m => m.Visibility == MembershipVisibility.Public);

        //获取访问者的所属组织Id列表。
        var visitorMemberOfOrgIds = from org in store.Organizations
                                    from member in org.Members
                                    where member.PersonId == personId
                                    select org.Id;

        return members.Where(m =>
            m.Visibility >= MembershipVisibility.AuthenticatedUser || m.Visibility == MembershipVisibility.Private &&
                                                                       visitorMemberOfOrgIds
                                                                           .Contains(m.OrganizationId));
    }
}