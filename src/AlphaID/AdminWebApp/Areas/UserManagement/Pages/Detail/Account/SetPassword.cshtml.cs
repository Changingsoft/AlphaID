using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class SetPasswordModel(NaturalPersonService naturalPersonService, UserManager<ApplicationUser> userManager) : PageModel
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
        ApplicationUser? person = await userManager.FindByIdAsync(anchor);
        return person == null
            ? NotFound()
            : await userManager.HasPasswordAsync(person)
                ? throw new InvalidOperationException("用户已具有密码，无法手动添加密码")
                : (IActionResult)Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        ApplicationUser? person = await userManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();

        if (await userManager.HasPasswordAsync(person)) throw new InvalidOperationException("用户已具有密码，无法手动添加密码");

        IdentityResult result = await naturalPersonService.AddPasswordAsync(person, NewPassword);
        if (result.Succeeded) return RedirectToPage("SetPasswordSuccess", new { anchor });

        foreach (IdentityError error in result.Errors) ModelState.AddModelError("", error.Description);
        return Page();
    }
}