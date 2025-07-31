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
    public ActionResult<OrganizationInfoModel> GetAsync(string id)
    {
        var orgs = from organization in organizationStore.Organizations
            where organization.Id == id && organization.Enabled
            select new OrganizationInfoModel()
            {
                Name = organization.Name,
                Domicile = organization.Domicile,
                Representative = organization.Representative,
                ProfileUrl = Url.Page("/Index", new{area = "Organization", anchor = organization.Name}),
                LocationWkt = organization.Location.AsText(),
                UpdateAt = organization.WhenChanged.ToUnixTimeSeconds(),
            };
        var org = orgs.FirstOrDefault();
        if (org == null)
            return NotFound();

        return Ok(org);
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
    public IEnumerable<OrganizationSearchModel> Search(string q)
    {
        IQueryable<OrganizationSearchModel> searchResults = from organization in organizationStore.Organizations
            where organization.Name.Contains(q) && organization.Enabled
            select new OrganizationSearchModel()
            {
                Name = organization.Name,
                Domicile = organization.Domicile,
                Representative = organization.Representative
            };

        return searchResults.Take(20);
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
    [ApiExplorerSettings(IgnoreApi = true)]
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

    public class OrganizationInfoModel
    {
        public string Name { get; set; } = null!;

        public string? Domicile { get; set; }

        public string? Representative { get; set; }

        public string? ProfileUrl { get; set; }

        public string? LocationWkt { get; set; }

        public long UpdateAt { get; set; }
    }

    public class OrganizationSearchModel
    {
        public string Name { get; set; } = null!;

        public string? Domicile { get; set; }

        public string? Representative { get; set; }
    }
}
