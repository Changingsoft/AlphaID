using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Account.Manage;

public class RemovePasswordModel : PageModel
{
    private readonly NaturalPersonManager userManager;

    public RemovePasswordModel(NaturalPersonManager userManager)
    {
        this.userManager = userManager;
    }

    [TempData]
    public string? StatusMessage { get; set; }

    [Display(Name = "����")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "{0}�Ǳ����")]
    [BindProperty]
    public string Password { get; set; } = default!;

    public IList<UserLoginInfo> Logins { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await this.userManager.GetUserAsync(this.User);
        return person == null ? this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await this.userManager.GetUserAsync(this.User);
        if (person == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }
        this.Logins = await this.userManager.GetLoginsAsync(person);
        if (!this.Logins.Any())
        {
            this.StatusMessage = "�������Ƴ����룬��Ϊû���κ��ⲿ��¼���ã��Ƴ������������ȫ�޷�ʹ�ø��˻���Ҫ�Ƴ����룬���������һ���ⲿ��¼��";
            return this.Page();
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        //check password
        if (!await this.userManager.CheckPasswordAsync(person, this.Password))
        {
            this.ModelState.AddModelError(nameof(this.Password), "�������");
            return this.Page();
        }

        var result = await this.userManager.RemovePasswordAsync(person);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                this.ModelState.AddModelError("", error.Description);
            return this.Page();
        }

        person.PasswordLastSet = null;
        await this.userManager.UpdateAsync(person);

        this.StatusMessage = "���ѳɹ��Ƴ����롣";
        this.Password = string.Empty;
        return this.Page();
    }
}
