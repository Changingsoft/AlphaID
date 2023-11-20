using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings
{
    public class RenameModel : PageModel
    {
        readonly OrganizationManager manager;

        public RenameModel(OrganizationManager manager)
        {
            this.manager = manager;
        }

        [BindProperty]
        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!this.manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!this.manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            if (this.manager.Organizations.Any(o => o.Name == this.Name))
                this.ModelState.AddModelError(nameof(this.Name), "The name is in use.");

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await this.manager.ChangeNameAsync(organization, this.Name, DateOnly.FromDateTime(DateTime.UtcNow), true);
            if (!this.Result.Succeeded)
                return this.Page();

            //Redirect with new name
            return this.RedirectToPage("/Index", new { anchor = this.Name });
        }
    }
}
