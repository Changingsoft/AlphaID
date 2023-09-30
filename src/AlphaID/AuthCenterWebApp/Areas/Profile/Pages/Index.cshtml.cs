using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Profile.Pages;

public class IndexModel : PageModel
{
    private readonly NaturalPersonManager manager;

    public IndexModel(NaturalPersonManager manager)
    {
        this.manager = manager;
    }

    public NaturalPerson Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await this.manager.GetUserAsync(this.HttpContext.User);
        if (person == null) { return this.NotFound(); }
        this.Data = person;
        return this.Page();
    }
}
