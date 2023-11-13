using Microsoft.AspNetCore.Authorization;

namespace AuthCenterWebApp.Services.Authorization;

/// <summary>
/// 表达需要组织的所有者的需求。
/// </summary>
public class OrganizationOwnerRequirement : IAuthorizationRequirement
{ }
