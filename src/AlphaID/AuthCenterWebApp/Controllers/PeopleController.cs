using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController(IUserStore<NaturalPerson> store) : ControllerBase
{
    public IQueryableUserStore<NaturalPerson> QueryableUserStore { get; set; } = store as IQueryableUserStore<NaturalPerson> ?? throw new NotSupportedException("不支持查询用户。");

    /// <summary>
    /// Get user's avatar picture.
    /// </summary>
    /// <param name="anchor">Anchor of user, always be user name.</param>
    /// <returns>image of user. if not exists, always return no-picture-avatar.png</returns>
    [AllowAnonymous]
    [HttpGet("/People/{anchor}/Avatar")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public ActionResult GetAvatarPicture(string anchor)
    {
        var profilePicture = (from user in QueryableUserStore.Users.AsNoTracking()
                              where user.UserName == anchor
                              select user.ProfilePicture).FirstOrDefault();
        if (profilePicture != null)
        {
            return File(profilePicture.Data, profilePicture.MimeType);
        }
        return File("~/img/no-picture-avatar.png", "image/png");
    }
}
