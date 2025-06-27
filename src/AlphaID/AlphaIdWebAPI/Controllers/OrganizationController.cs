using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AlphaIdWebAPI.Controllers;

/// <summary>
/// 组织信息。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="organizationStore"></param>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrganizationController(IOrganizationStore organizationStore) : ControllerBase
{
    /// <summary>
    /// 获取组织信息。
    /// </summary>
    /// <param name="id">组织的SubjectId</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationModel>> GetAsync(string id)
    {
        Organization? org = await organizationStore.FindByIdAsync(id);
        return org == null ? NotFound() : new OrganizationModel(org);
    }

    /// <summary>
    /// 给定关键字查找组织。
    /// </summary>
    /// <remarks>
    /// 支持通过登记的统一社会信用代码、组织机构代码、组织名称的一部分进行查找。
    /// </remarks>
    /// <param name="q">关键字</param>
    /// <returns></returns>
    [HttpGet("Suggestions")]
    [AllowAnonymous]
    [EnableRateLimiting("ip-fixed")]
    public IEnumerable<OrganizationModel> Search(string q)
    {
        IQueryable<Organization> searchResults =
            organizationStore.Organizations.Where(p => p.Name.Contains(q) && p.Enabled);

        IQueryable<OrganizationModel> result = searchResults.Take(50).Select(p => new OrganizationModel(p));
        return result;
    }

    /// <summary>
    /// Organization.
    /// </summary>
    /// <param name="SubjectId">Id</param>
    /// <param name="Name">名称。</param>
    /// <param name="Domicile">住所。</param>
    /// <param name="Contact">联系方式。</param>
    /// <param name="LegalPersonName">组织的负责人或代表人名称。</param>
    public record OrganizationModel(
        string SubjectId,
        string Name,
        string? Domicile,
        string? Contact,
        string? LegalPersonName)
    {
        /// <summary>
        /// </summary>
        /// <param name="organization"></param>
        public OrganizationModel(Organization organization)
            : this(organization.Id,
                organization.Name,
                organization.Domicile,
                organization.Contact,
                organization.Representative)
        {
        }
    }
}