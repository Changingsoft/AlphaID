using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class RemovePasswordModel(NaturalPersonManager userManager) : PageModel
{
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Validate_Required")]
    [BindProperty]
    public string Password { get; set; } = default!;

    public IList<UserLoginInfo> Logins { get; set; } = default!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await userManager.GetUserAsync(this.User);
        return person == null ? this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await userManager.GetUserAsync(this.User);
        if (person == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }
        this.Logins = await userManager.GetLoginsAsync(person);
        if (!this.Logins.Any())
        {
            this.ModelState.AddModelError("", "�������Ƴ����룬��Ϊû���κ��ⲿ��¼���ã��Ƴ������������ȫ�޷�ʹ�ø��˻���Ҫ�Ƴ����룬���������һ���ⲿ��¼��");
            this.Result = IdentityResult.Failed(new IdentityError()
            {
                Code = "Cannot remove password",
                Description = "�������Ƴ����룬��Ϊû���κ��ⲿ��¼���ã��Ƴ������������ȫ�޷�ʹ�ø��˻���Ҫ�Ƴ����룬���������һ���ⲿ��¼��",
            });
            return this.Page();
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        //check password
        if (!await userManager.CheckPasswordAsync(person, this.Password))
        {
            this.ModelState.AddModelError(nameof(this.Password), "�������");
            return this.Page();
        }

        this.Result = await userManager.RemovePasswordAsync(person);
        this.Password = string.Empty;
        return this.Page();
    }
}
