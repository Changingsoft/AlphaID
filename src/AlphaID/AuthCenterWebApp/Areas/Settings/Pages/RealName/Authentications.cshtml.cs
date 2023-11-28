using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class AuthenticationsModel : PageModel
    {
        RealNameManager realNameManager;
        NaturalPersonManager naturalPersonManager;

        public AuthenticationsModel(RealNameManager realNameManager, NaturalPersonManager naturalPersonManager)
        {
            this.realNameManager = realNameManager;
            this.naturalPersonManager = naturalPersonManager;
        }

        public IEnumerable<RealNameAuthentication> Authentications { get; set; } = Enumerable.Empty<RealNameAuthentication>();

        public async Task<IActionResult> OnGet()
        {
            var person = await this.naturalPersonManager.GetUserAsync(this.User);
            if (person == null)
                return this.NotFound();

            this.Authentications = this.realNameManager.GetAuthentications(person);
            return this.Page();
        }
    }
}
