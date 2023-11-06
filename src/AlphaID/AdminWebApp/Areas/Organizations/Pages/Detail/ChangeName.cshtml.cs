using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class ChangeNameModel : PageModel
{
    private readonly OrganizationManager manager;

    public ChangeNameModel(OrganizationManager manager)
    {
        this.manager = manager;
    }

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var org = await this.manager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();

        this.Input = new()
        {
            CurrentName = org.Name,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var org = await this.manager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();

        var result = await this.manager.ChangeNameAsync(org, this.Input.NewName, DateOnly.FromDateTime(this.Input.ChangeDate), this.Input.RecordUsedName, this.Input.ApplyChangeWhenNameDuplicated);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }

        return this.RedirectToPage("Index", new { id = this.Anchor });
    }

    public class InputModel
    {
        [Display(Name = "Current name")]
        public string CurrentName { get; init; } = default!;

        [Display(Name = "New name")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        public string NewName { get; init; } = default!;

        [DataType(DataType.Date)]
        [Display(Name = "When changed")]
        public DateTime ChangeDate { get; init; } = DateTime.Now;

        [Display(Name = "Record used name")]
        public bool RecordUsedName { get; init; } = true;

        [Display(Name = "Apply changes when name duplicated")]
        public bool ApplyChangeWhenNameDuplicated { get; init; } = false;
    }
}
