using AuthCenterWebApp.Areas.Organization;
using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace AuthCenterWebApp.Services.Authorization;

/// <summary>
/// 用于处理用户是否是组织的所有者的授权处理程序。
/// </summary>
public class OrganizationOwnerRequirementHandler : AuthorizationHandler<OrganizationOwnerRequirement>
{
    private readonly OrganizationManager organizationManager;
    private readonly OrganizationMemberManager memberManager;
    private readonly NaturalPersonManager personManager;

    public OrganizationOwnerRequirementHandler(OrganizationManager organizationManager, OrganizationMemberManager memberManager, NaturalPersonManager personManager)
    {
        this.organizationManager = organizationManager;
        this.memberManager = memberManager;
        this.personManager = personManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrganizationOwnerRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        Debug.Assert(httpContext != null);

        var person = await this.personManager.GetUserAsync(context.User);
        if (person == null)
            return;

        var anchor = httpContext.GetRouteValue("anchor")?.ToString();
        if (anchor == null)
            return;

        if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
            return;
        if (organization == null)
            return;

        var member = await this.memberManager.GetMemberAsync(person, organization);
        if (member == null)
            return;

        if (member.IsOwner)
        {
            context.Succeed(requirement);
        }
    }
}
