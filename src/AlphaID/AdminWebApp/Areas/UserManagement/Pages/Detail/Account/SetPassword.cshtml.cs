using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class SetPasswordModel(NaturalPersonManager userManager) : PageModel
{
    [StringLength(30, ErrorMessage = "Validate_StringLength")]
    [DataType(DataType.Password)]
    [BindProperty]
    public string NewPassword { get; set; } = null!;

    [StringLength(30, ErrorMessage = "Validate_StringLength")]
    [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
    [DataType(DataType.Password)]
    [BindProperty]
    public string ConfirmPassword { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        return person == null
            ? NotFound()
            : await userManager.HasPasswordAsync(person)
                ? throw new InvalidOperationException("�û��Ѿ������룬�޷��ֶ��������")
                : (IActionResult)Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();

        if (await userManager.HasPasswordAsync(person)) throw new InvalidOperationException("�û��Ѿ������룬�޷��ֶ��������");

        IdentityResult result = await userManager.AddPasswordAsync(person, NewPassword);
        if (result.Succeeded) return RedirectToPage("SetPasswordSuccess", new { anchor });

        foreach (IdentityError error in result.Errors) ModelState.AddModelError("", error.Description);
        return Page();
    }
}