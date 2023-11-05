using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class IndexModel : PageModel
{
    private readonly NaturalPersonManager userManager;

    public IndexModel(NaturalPersonManager userManager)
    {
        this.userManager = userManager;
    }

    public NaturalPerson Data { get; set; } = default!;

    public IList<UserLoginInfo> ExternalLogins { get; set; } = default!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        var person = await this.userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        this.Data = person;
        this.ExternalLogins = await this.userManager.GetLoginsAsync(person);
        return this.Page();
    }

    public async Task<IActionResult> OnGetPhotoAsync(string anchor)
    {
        var person = await this.userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        if (person.Avatar != null)
            return this.File(person.Avatar.Data, person.Avatar.MimeType);
        return this.File("~/img/no-picture-avatar.png", "image/png");
    }

    public async Task<IActionResult> OnPostUpdateProfilePictureAsync(string anchor)
    {
        if (!this.Request.Form.Files.Any())
            return this.BadRequest();

        var file = this.Request.Form.Files[0];
        var person = await this.userManager.FindByIdAsync(anchor);
        Debug.Assert(person != null);

        using var stream = file.OpenReadStream();
        byte[] data = new byte[stream.Length];
        await stream.ReadAsync(data, 0, data.Length);
        var result = await this.userManager.SetProfilePictureAsync(person, file.ContentType, data);
        if (result.Succeeded)
            return new JsonResult(true);
        else
            return new JsonResult("Can not update profile picture.");
    }

    public async Task<IActionResult> OnPostClearProfilePictureAsync(string anchor)
    {
        var person = await this.userManager.FindByIdAsync(anchor);
        if (person == null)
            return this.NotFound();

        this.Data = person;
        this.ExternalLogins = await this.userManager.GetLoginsAsync(person);

        person.Avatar = null;
        this.Result = await this.userManager.ClearProfilePictureAsync(person);
        return this.Page();
    }
}
