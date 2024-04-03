using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        NaturalPerson? person = await userManager.GetUserAsync(User);
        return person == null ? NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.") : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? person = await userManager.GetUserAsync(User);
        if (person == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        Logins = await userManager.GetLoginsAsync(person);
        if (!Logins.Any())
        {
            ModelState.AddModelError("", "�������Ƴ����룬��Ϊû���κ��ⲿ��¼���ã��Ƴ������������ȫ�޷�ʹ�ø��˻���Ҫ�Ƴ����룬���������һ���ⲿ��¼��");
            Result = IdentityResult.Failed(new IdentityError
            {
                Code = "Cannot remove password",
                Description = "�������Ƴ����룬��Ϊû���κ��ⲿ��¼���ã��Ƴ������������ȫ�޷�ʹ�ø��˻���Ҫ�Ƴ����룬���������һ���ⲿ��¼��"
            });
            return Page();
        }

        if (!ModelState.IsValid)
            return Page();

        //check password
        if (!await userManager.CheckPasswordAsync(person, Password))
        {
            ModelState.AddModelError(nameof(Password), "�������");
            return Page();
        }

        Result = await userManager.RemovePasswordAsync(person);
        Password = string.Empty;
        return Page();
    }
}