using AlphaIDWebAPI.Models;
using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIDWebAPI.Controllers;

/// <summary>
/// 组织信息。
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public partial class OrganizationController : ControllerBase
{
    private readonly IOrganizationStore organizationStore;
    private readonly OrganizationMemberManager memberManager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="organizationStore"></param>
    /// <param name="memberManager"></param>
    public OrganizationController(IOrganizationStore organizationStore, OrganizationMemberManager memberManager)
    {
        this.organizationStore = organizationStore;
        this.memberManager = memberManager;
    }

    /// <summary>
    /// 获取组织信息。
    /// </summary>
    /// <param name="id">组织的SubjectId</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationModel>> GetAsync(string id)
    {
        var org = await this.organizationStore.FindByIdAsync(id);
        return org == null ? this.NotFound() : new OrganizationModel(org);
    }

    /// <summary>
    /// 获取组织的成员。
    /// </summary>
    /// <param name="id">组织的SubjectId</param>
    /// <returns></returns>
    [HttpGet("{id}/Members")]
    public async Task<IEnumerable<OrganizationMemberModel>> GetMembersAsync(string id)
    {
        var org = await this.organizationStore.FindByIdAsync(id);
        if (org == null)
            return Enumerable.Empty<OrganizationMemberModel>();
        var members = await this.memberManager.GetMembersAsync(org);

        return from member in members select new OrganizationMemberModel(member);
    }

    /// <summary>
    /// 给定关键字查找组织。
    /// </summary>
    /// <remarks>
    /// 支持通过登记的统一社会信用代码、组织机构代码、组织名称的一部分进行查找。
    /// </remarks>
    /// <param name="keywords">关键字</param>
    /// <returns></returns>
    [HttpGet("Search/{keywords}")]
    [AllowAnonymous]
    public OrganizationSearchResult Search(string keywords)
    {
        var searchResults = this.organizationStore.Organizations.Where(p => p.Name.Contains(keywords) && p.Enabled);

        var result = new OrganizationSearchResult(searchResults.Take(50).Select(p => new OrganizationModel(p)), searchResults.Count() > 50);
        return result;
    }
}
