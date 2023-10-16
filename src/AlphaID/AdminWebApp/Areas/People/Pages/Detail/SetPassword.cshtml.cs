using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class SetPasswordModel : PageModel
{
    private readonly NaturalPersonManager userManager;

    public SetPasswordModel(NaturalPersonManager userManager)
    {
        this.userManager = userManager;
    }

    [StringLength(30, ErrorMessage = "Validate_StringLength")]
    [DataType(DataType.Password)]
    [BindProperty]
    public string NewPassword { get; set; } = default!;

    [StringLength(30, ErrorMessage = "Validate_StringLength")]
    [Compare(nameof(NewPassword), ErrorMessage = "Validate_PasswordConfirm")]
    [DataType(DataType.Password)]
    [BindProperty]
    public string ConfirmPassword { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var person = await this.userManager.FindByIdAsync(id);
        return person == null
            ? this.NotFound()
            : await this.userManager.HasPasswordAsync(person) ? throw new InvalidOperationException("用户已具有密码，无法手动添加密码") : (IActionResult)this.Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var person = await this.userManager.FindByIdAsync(id);
        if (person == null)
        {
            return this.NotFound();
        }

        if (await this.userManager.HasPasswordAsync(person))
        {
            throw new InvalidOperationException("用户已具有密码，无法手动添加密码");
        }

        var result = await this.userManager.AddPasswordAsync(person, this.NewPassword);
        if (result.Succeeded)
        {
            return this.RedirectToPage("SetPasswordSuccess", new { id });
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError("", error.Description);
        }
        return this.Page();
    }
}
