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
            this.ModelState.AddModelError("", "�Ѵ��ڴ����Ƶ���֯");
        }
        if (!string.IsNullOrEmpty(this.Input.USCI) && this.manager.Organizations.Any(p => p.USCI == this.Input.USCI))
            this.ModelState.AddModelError("", "ͳһ������ô����ѱ�ע��");

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
        //��Ϊ����֯�ĳ�Ա
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
        return USCC.TryParse(this.Input.USCI!, out _) ? new JsonResult(true) : (IActionResult)new JsonResult("ͳһ������ô�����Ч");
    }

    public class InputModel
    {
        [Display(Name = "��֯����", Description = "����д��֯����ʽ��������ȫ��", Prompt = "����д��֯����ʽ��������ȫ��")]
        [StringLength(50)]
        public string Name { get; set; } = default!;

        [Display(Name = "ͳһ������ô���")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "ͳһ������ô��������18λ")]
        [PageRemote(PageHandler = "ValidateUSCI", AdditionalFields = "__RequestVerificationToken", HttpMethod = "post")]
        public string? USCI { get; set; }

        [Display(Name = "ס��")]
        [StringLength(50)]
        public string? Domicile { get; set; }

        [Display(Name = "������")]
        [StringLength(20)]
        public string? Representative { get; set; }

        [Display(Name = "ְ��", Description = "���ڸ���֯�ڵ�ְ���ƺ�")]
        [StringLength(50)]
        public string? Title { get; set; }

        [Display(Name = "����", Description = "���ڸ���֯�Ĳ��Ż�칫��")]
        [StringLength(50)]
        public string? Department { get; set; }

        [Display(Name = "��ע", Description = "���ڸ���֯�ı�ע����")]
        [StringLength(50)]
        public string? Remark { get; set; }
    }
}
