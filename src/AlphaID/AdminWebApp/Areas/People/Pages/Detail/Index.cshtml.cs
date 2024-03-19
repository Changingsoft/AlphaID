using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class IndexModel(NaturalPersonManager userManager) : PageModel
{
    public NaturalPerson Data { get; set; } = default!;

    public IList<UserLoginInfo> ExternalLogins { get; set; } = default!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        var person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        this.Data = person;
        this.ExternalLogins = await userManager.GetLoginsAsync(person);
        return this.Page();
    }

    public async Task<IActionResult> OnGetPhotoAsync(string anchor)
    {
        var person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        if (person.ProfilePicture != null)
            return this.File(person.ProfilePicture.Data, person.ProfilePicture.MimeType);
        return this.File("~/img/no-picture-avatar.png", "image/png");
    }

    public async Task<IActionResult> OnPostUpdateProfilePictureAsync(string anchor)
    {
        if (!this.Request.Form.Files.Any())
            return this.BadRequest();

        var file = this.Request.Form.Files[0];
        var person = await userManager.FindByIdAsync(anchor);
        Debug.Assert(person != null);

        await using var stream = file.OpenReadStream();
        byte[] data = new byte[stream.Length];
        await stream.ReadAsync(data, 0, data.Length);
        var result = await userManager.SetProfilePictureAsync(person, file.ContentType, data);
        if (result.Succeeded)
            return new JsonResult(true);
        else
            return new JsonResult("Can not update profile picture.");
    }

    public async Task<IActionResult> OnPostClearProfilePictureAsync(string anchor)
    {
        var person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        this.Data = person;
        this.ExternalLogins = await userManager.GetLoginsAsync(person);

        person.ProfilePicture = null;
        this.Result = await userManager.ClearProfilePictureAsync(person);
        return this.Page();
    }
}
