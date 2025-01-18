using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages;

[BindProperties]
public class NewModel(OrganizationManager manager) : PageModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Validate_Required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Validate_Required")]
    [Display(Name = "Register with same name anyway")]
    public bool RegisterWithSameNameAnyway { get; set; }

    [Display(Name = "Domicile")]
    public string? Domicile { get; set; }

    [Display(Name = "Contact")]
    public string? Contact { get; set; }

    [Display(Name = "Legal person name")]
    public string? LegalPersonName { get; set; }

    [Display(Name = "Established at")]
    [DataType(DataType.Date)]
    public DateOnly? EstablishedAt { get; set; }

    [Display(Name = "Term begin")]
    [DataType(DataType.Date)]
    public DateOnly? TermBegin { get; set; }

    [Display(Name = "Term end")]
    [DataType(DataType.Date)]
    public DateOnly? TermEnd { get; set; }

    public IdOperationResult? OperationResult { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        IEnumerable<Organization> nameExists = manager.FindByName(Name);
        if (nameExists.Any())
            if (!RegisterWithSameNameAnyway)
            {
                ModelState.AddModelError(nameof(Name), "库中存在同名的组织，如果确实要注册，请勾选“即使名称相同，也要注册”复选框");
                return Page();
            }

        var factory = new OrganizationBuilder(Name);

        Organization org = factory.Organization;
        org.Domicile = Domicile;
        org.Representative = LegalPersonName;
        org.EstablishedAt = EstablishedAt;
        org.TermBegin = TermBegin;
        org.TermEnd = TermEnd;

        if (!ModelState.IsValid) return Page();

        try
        {
            IdOperationResult result = await manager.CreateAsync(org);
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