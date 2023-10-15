using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail;

public class EditModel : PageModel
{
    private readonly OrganizationManager manager;

    public EditModel(OrganizationManager manager)
    {
        this.manager = manager;
    }

    public GenericOrganization Data { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;



    public async Task<IActionResult> OnGetAsync(string id)
    {
        var org = await this.manager.FindByIdAsync(id);
        if (org == null)
            return this.NotFound();

        this.Data = org;

        this.Input = new()
        {
            Domicile = org.Domicile,
            Contact = org.Contact,
            LegalPersonName = org.LegalPersonName,
            EstablishedAt = org.EstablishedAt,
            TermBegin = org.TermBegin,
            TermEnd = org.TermEnd,
        };

        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (!this.ModelState.IsValid)
            return this.Page();

        var org = await this.manager.FindByIdAsync(id);
        if (org == null)
            return this.NotFound();

        org.Domicile = this.Input.Domicile;
        org.Contact = this.Input.Contact;
        org.LegalPersonName = this.Input.LegalPersonName;
        org.EstablishedAt = this.Input.EstablishedAt;
        org.TermBegin = this.Input.TermBegin;
        org.TermEnd = this.Input.TermEnd;

        try
        {
            await this.manager.UpdateAsync(org);
            return this.RedirectToPage("Index", new { id });
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }
    }

    public class InputModel
    {
        [Display(Name = "Domicile")]
        public string? Domicile { get; set; }

        [Display(Name = "Contact")]
        public string? Contact { get; set; }

        [Display(Name = "Legal person name")]
        public string? LegalPersonName { get; set; }

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
