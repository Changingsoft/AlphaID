using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizationController(IOrganizationStore organizationStore):ControllerBase
{
    [HttpGet("{anchor}/Picture")]
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
}
