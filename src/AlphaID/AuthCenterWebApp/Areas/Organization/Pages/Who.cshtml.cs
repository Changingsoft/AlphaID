using IDSubjects;
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

        public IEnumerable<GenericOrganization> Organizations { get; set; } = default!;

        public IActionResult OnGet(string anchor)
        {
            this.Organizations = this.organizationManager.SearchByName(anchor).ToList();
            if (!this.Organizations.Any())
                return this.NotFound();

            if (this.Organizations.Count() == 1)
            {
                return this.RedirectToPage("Index", new { anchor = this.Organizations.Single().Name });
            }
            return this.Page();
        }
    }
}
