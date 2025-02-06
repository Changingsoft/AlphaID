using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class LoginFailedModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
