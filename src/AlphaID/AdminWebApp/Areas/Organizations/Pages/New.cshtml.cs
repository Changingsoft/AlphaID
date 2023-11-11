using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages;

[BindProperties]
public class NewModel : PageModel
{
    private readonly OrganizationManager manager;
    private readonly IOrganizationStore organizationStore;

    public NewModel(OrganizationManager manager, IOrganizationStore organizationStore)
    {
        this.manager = manager;
        this.organizationStore = organizationStore;
    }

    [Display(Name = "Unified social credit code")]
    public string? Usci { get; set; }

    [Display(Name = "Name")]
    [Required(ErrorMessage = "Validate_Required")]
    public string Name { get; set; } = default!;

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
        if (!string.IsNullOrWhiteSpace(this.Usci))
        {
            if (!UnifiedSocialCreditCode.TryParse(this.Usci, out UnifiedSocialCreditCode uscc))
                this.ModelState.AddModelError(nameof(this.Usci), "ͳһ������ô��벻��ȷ��");

            var usciExists = this.organizationStore.Organizations.Any(p => p.Usci == uscc.ToString());
            if (usciExists)
                this.ModelState.AddModelError(nameof(this.Usci), "ͳһ������ô����ѱ��Ǽǡ�");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        var nameExists = this.manager.SearchByName(this.Name);
        if (nameExists.Any())
        {
            if (!this.RegisterWithSameNameAnyway)
            {
                this.ModelState.AddModelError(nameof(this.Name), "���д���ͬ������֯�����ȷʵҪע�ᣬ�빴ѡ����ʹ������ͬ��ҲҪע�ᡱ��ѡ��");
                return this.Page();
            }
        }

        var factory = new OrganizationBuilder(this.Name);
        if (!string.IsNullOrWhiteSpace(this.Usci))
        {
            if (UnifiedSocialCreditCode.TryParse(this.Usci, out UnifiedSocialCreditCode uscc))
            {
                factory.SetUsci(uscc);
            }
            else
            {
                this.ModelState.AddModelError(nameof(this.Usci), "ͳһ������ô��벻��ȷ��");
            }
        }

        var org = factory.Organization;
        org.Domicile = this.Domicile;
        org.Representative = this.LegalPersonName;
        org.EstablishedAt = this.EstablishedAt;
        org.TermBegin = this.TermBegin;
        org.TermEnd = this.TermEnd;

        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        try
        {
            var result = await this.manager.CreateAsync(org);
            if (result.Succeeded)
                return this.RedirectToPage("Detail/Index", new { anchor = org.Id });

            this.OperationResult = result;
            return this.Page();
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }
    }
}
