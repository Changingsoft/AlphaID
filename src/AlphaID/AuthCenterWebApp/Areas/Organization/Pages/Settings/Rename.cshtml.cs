using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings
{
    public class RenameModel(OrganizationManager manager) : PageModel
    {
        [BindProperty]
        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "Validate_StringLength")]
        public string Name { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            if (manager.Organizations.Any(o => o.Name == this.Name))
                this.ModelState.AddModelError(nameof(this.Name), "The name is in use.");

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await manager.ChangeNameAsync(organization, this.Name, DateOnly.FromDateTime(DateTime.UtcNow), true);
            if (!this.Result.Succeeded)
                return this.Page();

            //Redirect with new name
            return this.RedirectToPage("/Index", new { anchor = this.Name });
        }
    }
}
