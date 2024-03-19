using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class ChangeNameModel(OrganizationManager manager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var org = await manager.FindByIdAsync(this.Anchor);
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
        var org = await manager.FindByIdAsync(this.Anchor);
        if (org == null)
            return this.NotFound();

        var result = await manager.ChangeNameAsync(org, this.Input.NewName, DateOnly.FromDateTime(this.Input.ChangeDate), this.Input.RecordUsedName, this.Input.ApplyChangeWhenNameDuplicated);
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
        [Required(ErrorMessage = "Validate_Required")]
        public string CurrentName { get; set; } = default!;

        [Display(Name = "New name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        public string NewName { get; set; } = default!;

        [DataType(DataType.Date)]
        [Display(Name = "When changed")]
        public DateTime ChangeDate { get; set; } = DateTime.Now;

        [Display(Name = "Record used name")]
        public bool RecordUsedName { get; set; } = true;

        [Display(Name = "Apply changes when name duplicated")]
        public bool ApplyChangeWhenNameDuplicated { get; set; } = false;
    }
}
