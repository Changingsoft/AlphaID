using Microsoft.AspNetCore.Authorization;

namespace AdminWebApp.Pages.Account;

[AllowAnonymous]
public class AccessDeniedModel : PageModel
{
    public void OnGet()
    {
    }
}