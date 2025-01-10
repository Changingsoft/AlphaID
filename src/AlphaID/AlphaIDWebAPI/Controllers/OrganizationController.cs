using AlphaIdPlatform.Security;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlphaIdWebAPI.Controllers;

/// <summary>
///     组织信息。
/// </summary>
/// <remarks>
/// </remarks>
/// <param name="organizationStore"></param>
/// <param name="memberManager"></param>
/// <param name="personManager"></param>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrganizationController(
    IOrganizationStore organizationStore,
    OrganizationMemberManager memberManager,
    NaturalPersonManager personManager) : ControllerBase
{
    /// <summary>
    ///     获取组织信息。
    /// </summary>
    /// <param name="id">组织的SubjectId</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationModel>> GetAsync(string id)
    {
        GenericOrganization? org = await organizationStore.FindByIdAsync(id);
        return org == null ? NotFound() : new OrganizationModel(org);
    }

    /// <summary>
    ///     获取组织的成员。
    /// </summary>
    /// <param name="id">组织的SubjectId</param>
    /// <returns></returns>
    [HttpGet("{id}/Members")]
    public async Task<IEnumerable<MemberModel>> GetMembersAsync(string id)
    {
        NaturalPerson? visitor = null;
        string? visitorSubjectId = User.SubjectId();
        if (visitorSubjectId != null)
            visitor = await personManager.FindByIdAsync(User.SubjectId()!);

        //todo 从令牌确定访问者。
        GenericOrganization? org = await organizationStore.FindByIdAsync(id);
        if (org == null)
            return [];
        IEnumerable<OrganizationMember> members = await memberManager.GetVisibleMembersAsync(org, visitor);

        return from member in members select new MemberModel(member);
    }

    /// <summary>
    ///     给定关键字查找组织。
    /// </summary>
    /// <remarks>
    ///     支持通过登记的统一社会信用代码、组织机构代码、组织名称的一部分进行查找。
    /// </remarks>
    /// <param name="q">关键字</param>
    /// <returns></returns>
    [HttpGet("Suggestions")]
    [AllowAnonymous]
    public IEnumerable<OrganizationModel> Search(string q)
    {
        IQueryable<GenericOrganization> searchResults =
            organizationStore.Organizations.Where(p => p.Name.Contains(q) && p.Enabled);

        IQueryable<OrganizationModel> result = searchResults.Take(50).Select(p => new OrganizationModel(p));
        return result;
    }

    /// <summary>
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="UserName"></param>
    /// <param name="Title"></param>
    /// <param name="Department"></param>
    /// <param name="Remarks"></param>
    public record MemberModel(
        string Name,
        string UserName,
        string? Title,
        string? Department,
        string? Remarks)
    {
        /// <summary>
        /// </summary>
        /// <param name="member"></param>
        public MemberModel(OrganizationMember member)
            : this(member.Person.PersonName.FullName, member.Person.UserName, member.Title, member.Department,
                member.Remark)
        {
        }
    }

    /// <summary>
    ///     GenericOrganization.
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
        public OrganizationModel(GenericOrganization organization)
            : this(organization.Id,
                organization.Name,
                organization.Domicile,
                organization.Contact,
                organization.Representative)
        {
        }
    }
}