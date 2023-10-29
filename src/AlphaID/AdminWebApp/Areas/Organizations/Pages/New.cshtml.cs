using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages;

[BindProperties]
public class NewModel : PageModel
{
    private readonly OrganizationManager manager;
    private readonly IQueryableOrganizationStore organizationStore;

    public NewModel(OrganizationManager manager, IQueryableOrganizationStore organizationStore)
    {
        this.manager = manager;
        this.organizationStore = organizationStore;
    }

    [Display(Name = "Unified social credit code")]
    public string? USCI { get; set; }

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
    public DateTime? EstablishedAt { get; set; }

    [Display(Name = "Term begin")]
    [DataType(DataType.Date)]
    public DateTime? TermBegin { get; set; }

    [Display(Name = "Term end", Prompt = "dd", Description = "weew")]
    [DataType(DataType.Date)]
    public DateTime? TermEnd { get; set; }

    public IdOperationResult? OperationResult { get; set; }
    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!string.IsNullOrWhiteSpace(this.USCI))
        {
            if (!USCC.TryParse(this.USCI, out USCC uscc))
                this.ModelState.AddModelError(nameof(this.USCI), "统一社会信用代码不正确。");

            var usciExists = await this.organizationStore.FindByIdentityAsync("统一社会信用代码", uscc.ToString());
            if (usciExists != null)
                this.ModelState.AddModelError(nameof(this.USCI), "统一社会信用代码已被登记。");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        var nameExists = await this.manager.SearchByNameAsync(this.Name);
        if (nameExists.Any())
        {
            if (!this.RegisterWithSameNameAnyway)
            {
                this.ModelState.AddModelError(nameof(this.Name), "库中存在同名的组织，如果确实要注册，请勾选“即使名称相同，也要注册”复选框");
                return this.Page();
            }
        }

        var factory = new OrganizationBuilder(this.Name);
        if (!string.IsNullOrWhiteSpace(this.USCI))
        {
            if (USCC.TryParse(this.USCI, out USCC uscc))
            {
                factory.SetUSCI(uscc);
            }
            else
            {
                this.ModelState.AddModelError(nameof(this.USCI), "统一社会信用代码不正确。");
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
