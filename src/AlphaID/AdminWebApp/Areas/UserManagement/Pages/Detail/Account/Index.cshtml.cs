using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class IndexModel(UserManager<NaturalPerson> userManager) : PageModel
{
    public NaturalPerson Data { get; set; } = null!;

    public bool HasPassword { get; set; }

    public string? OperationResultMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();

        Data = person;
        HasPassword = await userManager.HasPasswordAsync(Data);

        return Page();
    }

}