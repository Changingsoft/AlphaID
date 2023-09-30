using IDSubjects;
using IDSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Profile.Pages.RealName;

public class IndexModel : PageModel
{
    private readonly NaturalPersonManager personManager;
    private readonly ChineseIDCardManager realNameValidator;

    public IndexModel(ChineseIDCardManager realNameValidator, NaturalPersonManager personManager)
    {
        this.realNameValidator = realNameValidator;
        this.personManager = personManager;
    }

    public NaturalPerson Person { get; set; } = default!;

    public ChineseIDCardValidation? CurrentValidation { get; set; }

    public ChineseIDCardValidation? PendingValidation { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var person = await this.personManager.GetUserAsync(this.HttpContext.User);
        if (person == null) { return this.BadRequest("can not load user by id"); }
        this.Person = person;
        this.CurrentValidation = await this.realNameValidator.GetCurrentAsync(person);
        this.PendingValidation = await this.realNameValidator.GetPendingRequestAsync(person);
        return this.Page();
    }
}
