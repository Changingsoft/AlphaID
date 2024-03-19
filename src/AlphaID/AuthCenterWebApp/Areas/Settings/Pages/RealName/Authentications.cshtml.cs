using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class AuthenticationsModel(RealNameManager realNameManager, NaturalPersonManager naturalPersonManager) : PageModel
    {
        public IEnumerable<RealNameAuthentication> Authentications { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await naturalPersonManager.GetUserAsync(this.User);
            if (person == null)
                return this.NotFound();

            this.Authentications = realNameManager.GetAuthentications(person);
            return this.Page();
        }
    }
}
