using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail;

public class ChangeNameModel(OrganizationManager manager) : PageModel
{

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();

        Input = new InputModel
        {
            CurrentName = org.Name,
            NewName = org.Name,
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        OrganizationOperationResult result = await manager.RenameAsync(org, Input.NewName, DateOnly.FromDateTime(Input.ChangeDate), Input.RecordUsedName);
        if (result.Succeeded) 
            return RedirectToPage("Index", new { anchor });

        foreach (string error in result.Errors) ModelState.AddModelError("", error);
        return Page();

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
        [Display(Name = "Change date")]
        public DateTime ChangeDate { get; set; } = DateTime.Now;

        [Display(Name = "Record used name")]
        public bool RecordUsedName { get; set; } = true;
    }
}