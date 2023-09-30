using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages;

[IgnoreAntiforgeryToken]
public class CreateModel : PageModel
{
    private readonly OrganizationManager manager;
    private readonly OrganizationMemberManager memberManager;
    private readonly NaturalPersonManager naturalPersonManager;

    public CreateModel(OrganizationManager manager, OrganizationMemberManager memberManager, NaturalPersonManager naturalPersonManager)
    {
        this.manager = manager;
        this.memberManager = memberManager;
        this.naturalPersonManager = naturalPersonManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }
        var name = this.Input.Name.Trim().Trim('\r', '\n').Replace(" ", string.Empty);
        if (this.manager.Organizations.Any(p => p.Name == name))
        {
            this.ModelState.AddModelError("", "已存在此名称的组织");
        }
        if (!string.IsNullOrEmpty(this.Input.USCI) && this.manager.Organizations.Any(p => p.USCI == this.Input.USCI))
            this.ModelState.AddModelError("", "统一社会信用代码已被注册");

        if (!this.ModelState.IsValid)
            return this.Page();

        OrganizationBuilder builder = new(name);
        if (!string.IsNullOrEmpty(this.Input.USCI))
            builder.SetUSCI(USCC.Parse(this.Input.USCI));

        var organization = builder.Organization;
        organization.Domicile = this.Input.Domicile;
        organization.LegalPersonName = this.Input.Representative;
        var result = await this.manager.CreateAsync(organization, this.User.GetPersonInfo());
        if (!result.IsSuccess)
        {
            foreach (var error in result.Errors)
            {
                this.ModelState.AddModelError("", error);
            }
            return this.Page();
        }
        //成为该组织的成员
        var person = await this.naturalPersonManager.GetUserAsync(this.User);
        if (person != null)
        {
            result = await this.memberManager.JoinOrganizationAsync(person, organization, this.Input.Title, this.Input.Department, this.Input.Remark);
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError("", error);
                }
                return this.Page();
            }
        }

        return this.RedirectToPage("Detail", new { id = organization.Id });


    }

    public IActionResult OnPostValidateUSCI(string usci)
    {
        return USCC.TryParse(this.Input.USCI!, out _) ? new JsonResult(true) : (IActionResult)new JsonResult("统一社会信用代码无效");
    }

    public class InputModel
    {
        [Display(Name = "组织名称", Description = "请填写组织的正式、完整的全称", Prompt = "请填写组织的正式、完整的全称")]
        [StringLength(50)]
        public string Name { get; set; } = default!;

        [Display(Name = "统一社会信用代码")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "统一社会信用代码必须是18位")]
        [PageRemote(PageHandler = "ValidateUSCI", AdditionalFields = "__RequestVerificationToken", HttpMethod = "post")]
        public string? USCI { get; set; }

        [Display(Name = "住所")]
        [StringLength(50)]
        public string? Domicile { get; set; }

        [Display(Name = "代表人")]
        [StringLength(20)]
        public string? Representative { get; set; }

        [Display(Name = "职务", Description = "您在该组织内的职务或称呼")]
        [StringLength(50)]
        public string? Title { get; set; }

        [Display(Name = "部门", Description = "您在该组织的部门或办公室")]
        [StringLength(50)]
        public string? Department { get; set; }

        [Display(Name = "备注", Description = "您在该组织的备注事项")]
        [StringLength(50)]
        public string? Remark { get; set; }
    }
}
