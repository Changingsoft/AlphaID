using AlphaIdPlatform.Security;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthCenterWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(policy: "RequireMembershipScope")]
public class MembershipController(IOrganizationStore store) : ControllerBase
{
    /// <summary>
    /// 获取用户的组织成员信息。
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<IEnumerable<MembershipModel>> GetMemberships()
    {
        var userId = User.SubjectId();
        if (userId == null)
            return Forbid();

        var memberships = from organization in store.Organizations
            from member in organization.Members
            where member.PersonId == userId
            select new MembershipModel()
            {
                Department = member.Department,
                Title = member.Title,
                OrganizationName = organization.Name,
                OrganizationId = organization.Id
            };
        return Ok(memberships);
    }

    /// <summary>
    /// 组织成员信息。
    /// </summary>
    public class MembershipModel
    {
        /// <summary>
        /// 职务或称呼。
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 部门。
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// 组织名称。
        /// </summary>
        public string OrganizationName { get; set; } = null!;

        /// <summary>
        /// 组织Id。
        /// </summary>
        public string OrganizationId { get; set; } = null!;
    }
}
