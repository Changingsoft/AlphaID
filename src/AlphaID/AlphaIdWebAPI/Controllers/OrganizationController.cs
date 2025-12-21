using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Organizational;

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
    /// <remarks>
    /// <b>该接口已不再建议，请从AuthCenter调用接口。</b>
    /// </remarks>
    /// <param name="id">组织的SubjectId</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Obsolete("使用AuthCenter的接口")]
    public async Task<ActionResult<OrganizationModel>> GetAsync(string id)
    {
        Organization? org = await organizationStore.FindByIdAsync(id);
        return org == null ? NotFound() : new OrganizationModel(org);
    }

    /// <summary>
    /// 给定关键字查找组织。
    /// </summary>
    /// <remarks>
    /// <b>该接口已不再建议，请从AuthCenter调用接口。</b>
    /// 支持通过登记的统一社会信用代码、组织机构代码、组织名称的一部分进行查找。
    /// </remarks>
    /// <param name="q">关键字</param>
    /// <returns></returns>
    [HttpGet("Suggestions")]
    [AllowAnonymous]
    [EnableRateLimiting("ip-fixed")]
    [Obsolete("使用AuthCenter的接口")]
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
    public class OrganizationModel
    {
        /// <summary>
        /// </summary>
        /// <param name="organization"></param>
        public OrganizationModel(Organization organization)
        {
            SubjectId = organization.Id;
            Name = organization.Name;
            Domicile = organization.Domicile;
            Contact = organization.Contact;
            LegalPersonName = organization.Representative;
            USCC = organization.USCC;
            DUNS = organization.DUNS;
            LEI = organization.LEI;
        }

        /// <summary>
        /// Id
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 住所。
        /// </summary>
        public string? Domicile { get; set; }

        /// <summary>
        /// 联系方式。
        /// </summary>
        public string? Contact { get; set; }

        /// <summary>
        /// 组织的负责人或代表人名称。
        /// </summary>
        public string? LegalPersonName { get; set; }

        /// <summary>
        /// USCC
        /// </summary>
        public string? USCC { get; set; }

        /// <summary>
        /// DUNS
        /// </summary>
        public string? DUNS { get; set; }


        /// <summary>
        /// LEI
        /// </summary>
        public string? LEI { get; set; }
    }
}