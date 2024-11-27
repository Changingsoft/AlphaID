using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail;

public class GeneralModel(OrganizationManager manager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IdOperationResult? OperationResult { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        GenericOrganization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();

        Input = new InputModel
        {
            Contact = org.Contact,
            Domicile = org.Domicile,
            Representative = org.Representative,
            Website = org.Website,
            EstablishedAt = org.EstablishedAt?.ToDateTime(TimeOnly.MinValue),
            Name = org.Name,
            TermBegin = org.TermBegin?.ToDateTime(TimeOnly.MinValue),
            TermEnd = org.TermEnd?.ToDateTime(TimeOnly.MinValue)
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        GenericOrganization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        org.Contact = Input.Contact;
        org.Domicile = Input.Domicile;
        org.Representative = Input.Representative;
        org.Website = Input.Website;
        org.EstablishedAt = Input.EstablishedAt.HasValue ? DateOnly.FromDateTime(Input.EstablishedAt.Value) : null;
        org.TermBegin = Input.TermBegin.HasValue ? DateOnly.FromDateTime(Input.TermBegin.Value) : null;
        org.TermEnd = Input.TermEnd.HasValue ? DateOnly.FromDateTime(Input.TermEnd.Value) : null;

        await manager.UpdateAsync(org);
        OperationResult = IdOperationResult.Success;
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string Name { get; set; } = default!;

        [Display(Name = "Domicile")]
        public string? Domicile { get; set; }

        [Display(Name = "Contact")]
        public string? Contact { get; set; }

        [Display(Name = "Representative")]
        public string? Representative { get; set; }

        [Display(Name = "Website")]
        public string? Website { get; set; }

        [Display(Name = "Established at")]
        [DataType(DataType.Date)]
        public DateTime? EstablishedAt { get; set; }

        [Display(Name = "Term begin")]
        [DataType(DataType.Date)]
        public DateTime? TermBegin { get; set; }

        [Display(Name = "Term end")]
        [DataType(DataType.Date)]
        public DateTime? TermEnd { get; set; }
    }
}