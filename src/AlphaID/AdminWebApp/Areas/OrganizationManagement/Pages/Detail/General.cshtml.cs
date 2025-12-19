using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail;

public class GeneralModel(OrganizationManager manager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    [BindProperty]
    [Display(Name = "USCC")]
    [StringLength(18, MinimumLength = 18, ErrorMessage = "Validate_StringLength")]
    public string? USCC { get; set; }

    [BindProperty]
    [Display(Name = "DUNS")]
    [StringLength(9, ErrorMessage = "Validate_StringLength")]
    public string? DUNS { get; set; }

    [BindProperty]
    [Display(Name = "LEI")]
    [StringLength(20, ErrorMessage = "Validate_StringLength")]
    public string? LEI { get; set; }

    public OrganizationOperationResult? OperationResult { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
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
        USCC = org.USCC;
        DUNS = org.DUNS;
        LEI = org.LEI;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        Organization? org = await manager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();
        UnifiedSocialCreditCode usci = default;
        if (USCC != null && !UnifiedSocialCreditCode.TryParse(USCC, out usci))
        {
            ModelState.AddModelError(nameof(USCC), "Validate_Organization_USCC_Invalid");
        }

        if (!ModelState.IsValid)
            return Page();

        org.Contact = Input.Contact;
        org.Domicile = Input.Domicile;
        org.Representative = Input.Representative;
        org.Website = Input.Website;
        org.EstablishedAt = Input.EstablishedAt.HasValue ? DateOnly.FromDateTime(Input.EstablishedAt.Value) : null;
        org.TermBegin = Input.TermBegin.HasValue ? DateOnly.FromDateTime(Input.TermBegin.Value) : null;
        org.TermEnd = Input.TermEnd.HasValue ? DateOnly.FromDateTime(Input.TermEnd.Value) : null;
        org.USCC = usci.ToString();
        org.DUNS = DUNS;
        org.LEI = LEI;

        await manager.UpdateAsync(org);
        OperationResult = OrganizationOperationResult.Success;
        return Page();
    }

    public class InputModel
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