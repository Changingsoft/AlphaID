using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using Organizational;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OrganizationManagement.Pages;

[BindProperties]
public class NewModel(OrganizationManager manager) : PageModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Validate_Required")]
    [StringLength(100, ErrorMessage = "Validate_StringLength")]
    public string Name { get; set; } = null!;

    [Display(Name = "Domicile")]
    [StringLength(100, ErrorMessage = "Validate_StringLength")]
    public string? Domicile { get; set; }

    [Display(Name = "Contact")]
    [StringLength(50, ErrorMessage = "Validate_StringLength")]
    public string? Contact { get; set; }

    [Display(Name = "Representative")]
    [StringLength(20, ErrorMessage = "Validate_StringLength")]
    public string? Representative { get; set; }

    public OrganizationOperationResult? OperationResult { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var org = new Organization(Name)
        {
            Domicile = Domicile,
            Representative = Representative,
        };

        if (!ModelState.IsValid) return Page();

        try
        {
            OrganizationOperationResult result = await manager.CreateAsync(org);
            if (result.Succeeded)
                return RedirectToPage("Detail/Index", new { anchor = org.Id });

            OperationResult = result;
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Page();
        }
    }
}