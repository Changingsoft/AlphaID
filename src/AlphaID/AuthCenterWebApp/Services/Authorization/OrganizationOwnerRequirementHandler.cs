using System.Diagnostics;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Subjects;
using AuthCenterWebApp.Areas.Organization;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AuthCenterWebApp.Services.Authorization;

/// <summary>
///     用于处理用户是否是组织的所有者的授权处理程序。
/// </summary>
public class OrganizationOwnerRequirementHandler(
    OrganizationManager organizationManager,
    OrganizationMemberManager memberManager,
    UserManager<NaturalPerson> personManager) : AuthorizationHandler<OrganizationOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        OrganizationOwnerRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        Debug.Assert(httpContext != null);

        NaturalPerson? person = await personManager.GetUserAsync(context.User);
        if (person == null)
            return;

        var anchor = httpContext.GetRouteValue("anchor")?.ToString();
        if (anchor == null)
            return;

        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out Organization? organization))
            return;
        if (organization == null)
            return;

        OrganizationMember? member = await memberManager.GetMemberAsync(person.Id, organization.Id);
        if (member == null)
            return;

        if (member.IsOwner) context.Succeed(requirement);
    }
}