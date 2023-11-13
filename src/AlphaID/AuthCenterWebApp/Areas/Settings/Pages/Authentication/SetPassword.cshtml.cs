#nullable disable

using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class SetPasswordModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly SignInManager<NaturalPerson> signInManager;

    public SetPasswordModel(
        NaturalPersonManager userManager,
        SignInManager<NaturalPerson> signInManager)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(100, ErrorMessage = "Validate_StringLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        var hasPassword = await this.userManager.HasPasswordAsync(user);

        return hasPassword ? this.RedirectToPage("./ChangePassword") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        var addPasswordResult = await this.userManager.AddPasswordAsync(user, this.Input.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            foreach (var error in addPasswordResult.Errors)
            {
                this.ModelState.AddModelError(string.Empty, error.Description);
            }
            return this.Page();
        }

        await this.signInManager.RefreshSignInAsync(user);
        this.StatusMessage = "您的密码已设置。";

        return this.RedirectToPage();
    }
}
