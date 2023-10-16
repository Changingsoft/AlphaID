using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class IndexModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly INaturalPersonImageStore imageStore;

    public IndexModel(NaturalPersonManager userManager, INaturalPersonImageStore imageStore)
    {
        this.userManager = userManager;
        this.imageStore = imageStore;
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
            return this.File("~/img/no-picture-avatar.png", "image/png");
        var img = await this.imageStore.GetPhotoAsync(person);
        return img == null ? this.File("~/img/no-picture-avatar.png", "image/png") : this.File(img.Value.ImageContent, img.Value.MimeType);
    }
}
