using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
    /// Retrieves the profile picture of an organization based on the specified anchor.
    /// </summary>
    /// <remarks>This method allows anonymous access and returns the profile picture as a file result.  If the
    /// organization does not have a profile picture, a default image is returned.</remarks>
    /// <param name="anchor">The unique identifier or name of the organization whose profile picture is to be retrieved.</param>
    /// <returns>An <see cref="ActionResult"/> containing the organization's profile picture if found; otherwise, a default
    /// image.</returns>
    [AllowAnonymous]
    [HttpGet("/Organization/{anchor}/Picture")]
    public ActionResult GetOrganizationProfilePicture(string anchor)
    {
        var profilePicture = (from organization in organizationStore.Organizations.AsNoTracking()
                              where organization.Name == anchor
                              select organization.ProfilePicture).FirstOrDefault();
        if (profilePicture != null)
        {
            return File(profilePicture.Data, profilePicture.MimeType);
        }
        return File("~/img/org-no-logo.png", "image/png");
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
