using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings
{
    public class DeleteModel : PageModel
    {
        OrganizationManager manager;

        public DeleteModel(OrganizationManager manager)
        {
            this.manager = manager;
        }

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!this.manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
            {
                return this.NotFound();
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!this.manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
            {
                return this.NotFound();
            }

            this.Result = await this.manager.DeleteAsync(organization);
            if (!this.Result.Succeeded)
            {
                return this.Page();
            }

            return this.RedirectToPage("/Index", new { area = "" });
        }
    }
}
