using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages
{
    public class WhoModel : PageModel
    {
        private readonly OrganizationManager organizationManager;

        public WhoModel(OrganizationManager organizationManager)
        {
            this.organizationManager = organizationManager;
        }

        public GenericOrganization[] Organizations { get; set; } = Array.Empty<GenericOrganization>();

        public IActionResult OnGet(string anchor)
        {
            //hack 可能导致性能问题。
            this.Organizations = this.organizationManager.FindByName(anchor).ToArray();
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
