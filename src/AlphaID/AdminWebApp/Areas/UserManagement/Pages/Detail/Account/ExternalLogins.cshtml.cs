using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class ExternalLoginsModel(UserManager<ApplicationUser> personManager) : PageModel
{
    public IEnumerable<UserLoginInfo> Logins { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        ApplicationUser? person = await personManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();

        Logins = await personManager.GetLoginsAsync(person);

        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string provider, string providerKey)
    {
        ApplicationUser? person = await personManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();

        IdentityResult result = await personManager.RemoveLoginAsync(person, provider, providerKey);
        if (!result.Succeeded)
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        return RedirectToPage();
    }
}