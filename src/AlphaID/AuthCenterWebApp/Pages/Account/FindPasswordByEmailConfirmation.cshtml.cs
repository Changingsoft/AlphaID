#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages.Account;

[AllowAnonymous]
public class FindPasswordByEmailConfirmation : PageModel
{
    public void OnGet()
    {
    }
}
