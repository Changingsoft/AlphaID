using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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


    public async Task<IActionResult> OnGetAsync(string id)
    {
        var person = await this.userManager.FindByIdAsync(id);
        if (person == null)
            return this.NotFound();

        this.Data = person;
        this.ExternalLogins = await this.userManager.GetLoginsAsync(person);
        return this.Page();
    }

    public async Task<IActionResult> OnGetPhotoAsync(string id)
    {
        var person = await this.userManager.FindByIdAsync(id);
        if (person == null)
            return this.NotFound();

        if (person.Avatar != null)
            return this.File(person.Avatar.Data, person.Avatar.MimeType);
        return this.File("~/img/no-picture-avatar.png", "image/png");
    }
}
