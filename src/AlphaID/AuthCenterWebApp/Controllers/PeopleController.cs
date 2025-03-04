using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthCenterWebApp.Controllers;

[Route("[controller]")]
[ApiController]
public class PeopleController(IUserStore<NaturalPerson> store) : ControllerBase
{
    public IQueryableUserStore<NaturalPerson> QueryableUserStore { get; set; } = store as IQueryableUserStore<NaturalPerson> ?? throw new NotSupportedException("不支持查询用户。");

    [HttpGet("{anchor}/Avatar")]
    public ActionResult GetAvatarPicture(string anchor)
    {
        var profilePicture = (from user in QueryableUserStore.Users.AsNoTracking()
                              where user.UserName == anchor || user.Id == anchor
                              select user.ProfilePicture).FirstOrDefault();
        if(profilePicture != null)
        {
            return File(profilePicture.Data, profilePicture.MimeType);
        }
        return File("~/img/no-picture-avatar.png", "image/png");
    }
}
