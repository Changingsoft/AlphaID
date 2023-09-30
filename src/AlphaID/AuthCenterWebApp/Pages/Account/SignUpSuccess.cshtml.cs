using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class SignUpSuccessModel : PageModel
{
    public void OnGet()
    {
    }
}
