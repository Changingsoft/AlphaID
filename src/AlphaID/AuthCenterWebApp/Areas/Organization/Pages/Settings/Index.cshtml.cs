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

        public OperationResult OperationResult { get; set; } = default!;

        public async Task<IActionResult> OnGet(string anchor)
        {
            var orgs = await this.manager.FindByAnchorAsync(anchor);
            if (!orgs.Any())
                return this.NotFound();
            if (orgs.Count() > 1)
                return this.RedirectToPage("/Who", new { anchor });
            var org = orgs.First();

            this.Input = new InputModel()
            {
                Description = org.Description,
            };

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var orgs = await this.manager.FindByAnchorAsync(anchor);
            if (!orgs.Any())
                return this.NotFound();
            if (orgs.Count() > 1)
                return this.RedirectToPage("/Who", new { anchor });
            var org = orgs.First();

            if (!this.ModelState.IsValid)
                return this.Page();

            org.Description = this.Input.Description;
            await this.manager.UpdateAsync(org);
            this.OperationResult = OperationResult.Success;
            return this.Page();
        }

        public class InputModel
        {
            [Display(Name = "Description")]
            public string? Description { get; set; }

        }
    }
}
