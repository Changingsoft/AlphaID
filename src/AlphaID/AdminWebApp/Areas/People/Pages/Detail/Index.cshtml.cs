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
            return NotFound();

        Data = person;
        ExternalLogins = await userManager.GetLoginsAsync(person);
        return Page();
    }

    public async Task<IActionResult> OnGetPhotoAsync(string anchor)
    {
        var person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        if (person.ProfilePicture != null)
            return File(person.ProfilePicture.Data, person.ProfilePicture.MimeType);
        return File("~/img/no-picture-avatar.png", "image/png");
    }

    public async Task<IActionResult> OnPostUpdateProfilePictureAsync(string anchor)
    {
        if (!Request.Form.Files.Any())
            return BadRequest();

        var file = Request.Form.Files[0];
        var person = await userManager.FindByIdAsync(anchor);
        Debug.Assert(person != null);

        await using var stream = file.OpenReadStream();
        var data = new byte[stream.Length];
        await stream.ReadAsync(data);
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
            return NotFound();

        Data = person;
        ExternalLogins = await userManager.GetLoginsAsync(person);

        person.ProfilePicture = null;
        Result = await userManager.ClearProfilePictureAsync(person);
        return Page();
    }
}
