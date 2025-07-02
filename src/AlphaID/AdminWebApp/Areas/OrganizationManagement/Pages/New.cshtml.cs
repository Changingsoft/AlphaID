using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OrganizationManagement.Pages;

[BindProperties]
public class NewModel(OrganizationManager manager) : PageModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Validate_Required")]
    public string Name { get; set; } = null!;

    [Display(Name = "Domicile")]
    public string? Domicile { get; set; }

    [Display(Name = "Contact")]
    public string? Contact { get; set; }

    [Display(Name = "Representative")]
    public string? Representative { get; set; }

    [Display(Name = "Established at")]
    [DataType(DataType.Date)]
    public DateOnly? EstablishedAt { get; set; }

    [Display(Name = "Term begin")]
    [DataType(DataType.Date)]
    public DateOnly? TermBegin { get; set; }

    [Display(Name = "Term end")]
    [DataType(DataType.Date)]
    public DateOnly? TermEnd { get; set; }

    public OrganizationOperationResult? OperationResult { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        Organization org = new Organization(Name)
        {
            Domicile = Domicile,
            Representative = Representative,
            EstablishedAt = EstablishedAt,
            TermBegin = TermBegin,
            TermEnd = TermEnd
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