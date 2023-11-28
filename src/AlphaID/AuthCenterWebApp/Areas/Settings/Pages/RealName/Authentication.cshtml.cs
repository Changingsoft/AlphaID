using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.RealName
{
    public class AuthenticationModel : PageModel
    {
        RealNameManager realNameManager;
        NaturalPersonManager naturalPersonManager;

        public AuthenticationModel(RealNameManager realNameManager, NaturalPersonManager naturalPersonManager)
        {
            this.realNameManager = realNameManager;
            this.naturalPersonManager = naturalPersonManager;
        }

        public RealNameAuthentication Data { get; set; } = default!;

        public async Task<IActionResult> OnGet(string id)
        {
            var person = await this.naturalPersonManager.GetUserAsync(this.User);
            if (person == null) { return this.NotFound(); }

            var authentications = this.realNameManager.GetAuthentications(person);
            var authentication = authentications.FirstOrDefault(a => a.Id == id);
            if (authentication == null) { return this.NotFound(); }

            this.Data = authentication;
            return this.Page();
        }
    }
}
