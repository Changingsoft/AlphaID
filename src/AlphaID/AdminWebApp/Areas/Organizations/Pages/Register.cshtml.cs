using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages;

[BindProperties]
public class RegisterModel : PageModel
{
    private readonly OrganizationManager manager;
    private readonly IQueryableOrganizationStore organizationStore;

    public RegisterModel(OrganizationManager manager, IQueryableOrganizationStore organizationStore)
    {
        this.manager = manager;
        this.organizationStore = organizationStore;
    }

    [Display(Name = "统一社会信用代码")]
    public string? USCI { get; set; }

    [Display(Name = "名称")]
    [Required(ErrorMessage = "{0}是必需的")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "{0}是必需的")]
    [Display(Name = "即使名称相同，也要注册")]
    public bool RegisterWithSameNameAnyway { get; set; }

    [Display(Name = "住所")]
    public string? Domicile { get; set; }

    [Display(Name = "联系电话")]
    public string? Contact { get; set; }

    [Display(Name = "法定代表人")]
    public string? LegalPersonName { get; set; }

    [Display(Name = "成立日期")]
    [DataType(DataType.Date)]
    public DateTime? EstablishedAt { get; set; }

    [Display(Name = "营业期起")]
    [DataType(DataType.Date)]
    public DateTime? TermBegin { get; set; }

    [Display(Name = "营业期至", Prompt = "dd", Description = "weew")]
    [DataType(DataType.Date)]
    public DateTime? TermEnd { get; set; }

    [Display(Name = "创建人")]
    public string? CreatorId { get; set; }


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
        org.LegalPersonName = this.LegalPersonName;
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
            return this.RedirectToPage("Detail/Index", new { id = org.Id });
        }
        catch (Exception ex)
        {
            this.ModelState.AddModelError("", ex.Message);
            return this.Page();
        }
    }
}
