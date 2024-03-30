using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail.Account
{
    public class ExternalLoginsModel(NaturalPersonManager personManager) : PageModel
    {
        public IEnumerable<UserLoginInfo> Logins { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var person = await personManager.FindByIdAsync(anchor);
            if (person == null) { return NotFound(); }

            Logins = await personManager.GetLoginsAsync(person);

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(string anchor, string provider, string providerKey)
        {
            var person = await personManager.FindByIdAsync(anchor);
            if (person == null) { return NotFound(); }

            var result = await personManager.RemoveLoginAsync(person, provider, providerKey);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return RedirectToPage();
        }
    }
}
