using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminWebApp.Pages
{
    [AllowAnonymous]
    public class SetLanguageModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost(string culture, string returnUrl)
        {
            this.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return this.Redirect(returnUrl);
        }
    }
}
