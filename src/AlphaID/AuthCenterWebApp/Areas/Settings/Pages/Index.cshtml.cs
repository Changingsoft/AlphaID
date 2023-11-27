using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            return this.RedirectToPage("Profile/Index");
        }
    }
}
