using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class IndexModel(ApplicationUserManager<NaturalPerson> userManager) : PageModel
{
    public NaturalPerson Data { get; set; } = null!;

    public IList<UserLoginInfo> ExternalLogins { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Data = person;
        ExternalLogins = await userManager.GetLoginsAsync(person);
        return Page();
    }

    public IActionResult OnGetPhotoAsync(string anchor)
    {
        var photo = userManager.Users.AsNoTracking().Where(u => u.Id == anchor).Select(u => u.ProfilePicture).FirstOrDefault();

        if (photo != null)
            return File(photo.Data, photo.MimeType);
        return File("~/img/no-picture-avatar.png", "image/png");
    }

    public async Task<IActionResult> OnPostUpdateProfilePictureAsync(string anchor)
    {
        if (!Request.Form.Files.Any())
            return BadRequest();

        IFormFile file = Request.Form.Files[0];
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        Debug.Assert(person != null);

        await using Stream stream = file.OpenReadStream();
        var data = new byte[stream.Length];
        await stream.ReadAsync(data);
        IdentityResult result = await userManager.SetProfilePictureAsync(person, file.ContentType, data);
        if (result.Succeeded)
            return new JsonResult(true);
        return new JsonResult("Can not update profile picture.");
    }

    public async Task<IActionResult> OnPostClearProfilePictureAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Data = person;
        ExternalLogins = await userManager.GetLoginsAsync(person);

        person.ProfilePicture = null;
        Result = await userManager.ClearProfilePictureAsync(person);
        return Page();
    }
}