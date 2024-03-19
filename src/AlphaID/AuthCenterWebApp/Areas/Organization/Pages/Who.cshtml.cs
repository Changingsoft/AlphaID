using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages
{
    public class WhoModel(OrganizationManager organizationManager) : PageModel
    {
        public GenericOrganization[] Organizations { get; set; } = [];

        public IActionResult OnGet(string anchor)
        {
            this.Organizations = organizationManager.FindByName(anchor).ToArray();
            if (this.Organizations.Length == 0)
                return this.NotFound();

            if (this.Organizations.Length == 1)
            {
                return this.RedirectToPage("Index", new { anchor = this.Organizations[0].Name });
            }
            return this.Page();
        }
    }
}
