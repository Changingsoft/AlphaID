using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail;

public class ChangeNameModel(OrganizationManager manager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        Organization? org = await manager.FindByIdAsync(Anchor);
        if (org == null)
            return NotFound();

        Input = new InputModel
        {
            CurrentName = org.Name
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Organization? org = await manager.FindByIdAsync(Anchor);
        if (org == null)
            return NotFound();

        OrganizationOperationResult result = await manager.ChangeNameAsync(org, Input.NewName,
            DateOnly.FromDateTime(Input.ChangeDate));
        if (!result.Succeeded)
        {
            foreach (string error in result.Errors) ModelState.AddModelError("", error);
            return Page();
        }

        return RedirectToPage("Index", new { id = Anchor });
    }

    public class InputModel
    {
        [Display(Name = "Current name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string CurrentName { get; set; } = null!;

        [Display(Name = "New name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        public string NewName { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "When changed")]
        public DateTime ChangeDate { get; set; } = DateTime.Now;

        [Display(Name = "Record used name")]
        public bool RecordUsedName { get; set; } = true;

        [Display(Name = "Apply changes when name duplicated")]
        public bool ApplyChangeWhenNameDuplicated { get; set; } = false;
    }
}