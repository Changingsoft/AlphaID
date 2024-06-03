using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Pages;

[AllowAnonymous]
public class TermsModel : PageModel
{
    public void OnGet()
    {
    }
}