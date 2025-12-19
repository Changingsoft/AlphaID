using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Organizational;
using System.Diagnostics;

namespace AuthCenterWebApp.Services.Authorization;

/// <summary>
/// 用于处理用户是否是组织的所有者的授权处理程序。
/// </summary>
public class OrganizationOwnerRequirementHandler(
    OrganizationManager organizationManager,
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

        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return;

        OrganizationMember? member = organization.Members.FirstOrDefault(m => m.PersonId == person.Id);
        if (member == null)
            return;

        if (member.IsOwner) context.Succeed(requirement);
    }
}