using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizationController(IOrganizationStore organizationStore) : ControllerBase
{
    /// <summary>
    /// Retrieves the profile picture of an organization based on the specified anchor.
    /// </summary>
    /// <remarks>This method allows anonymous access and returns the profile picture as a file result.  If the
    /// organization does not have a profile picture, a default image is returned.</remarks>
    /// <param name="anchor">The unique identifier or name of the organization whose profile picture is to be retrieved.</param>
    /// <returns>An <see cref="ActionResult"/> containing the organization's profile picture if found; otherwise, a default
    /// image.</returns>
    [AllowAnonymous]
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
