using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings
{
    public class IndexModel : PageModel
    {
        private readonly OrganizationManager manager;

        public IndexModel(OrganizationManager manager)
        {
            this.manager = manager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult OperationResult { get; set; } = default!;

        public IActionResult OnGet(string anchor)
        {
            if (!this.manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            this.Input = new InputModel()
            {
                Description = organization.Description,
                Domicile = organization.Domicile,
                Contact = organization.Contact,
                Representative = organization.Representative,
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!this.manager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            if (!this.ModelState.IsValid)
                return this.Page();

            organization.Description = this.Input.Description;
            organization.Domicile = this.Input.Domicile;
            organization.Contact = this.Input.Contact;
            organization.Representative = this.Input.Representative;

            await this.manager.UpdateAsync(organization);
            this.OperationResult = IdOperationResult.Success;
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Description")]
            public string? Description { get; init; }

            [Display(Name = "Domicile")]
            public string? Domicile { get; init; }

            [Display(Name = "Contact")]
            public string? Contact { get; init; }

            [Display(Name = "Representative")]
            public string? Representative { get; init; }

        }
    }
}
