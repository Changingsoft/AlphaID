using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings
{
    public class DeleteModel(OrganizationManager manager) : PageModel
    {
        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
            {
                return this.NotFound();
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
            {
                return this.NotFound();
            }

            this.Result = await manager.DeleteAsync(organization);
            if (!this.Result.Succeeded)
            {
                return this.Page();
            }

            return this.RedirectToPage("/Index", new { area = "" });
        }
    }
}
