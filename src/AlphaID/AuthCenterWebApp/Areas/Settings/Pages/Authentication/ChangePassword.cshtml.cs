using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class ChangePasswordModel(
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager,
    ILogger<ChangePasswordModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public NaturalPerson Person { get; set; } = default!;

    public IList<UserLoginInfo> ExternalLogins { get; set; } = default!;

    public IdentityResult? Result { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string CurrentPassword { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(100, ErrorMessage = "Validate_StringLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = default!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; } = default!;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }
        this.Person = user;
        this.ExternalLogins = await userManager.GetLoginsAsync(user);
        var hasPassword = await userManager.HasPasswordAsync(user);
        return !hasPassword ? this.RedirectToPage("./SetPassword") : this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }
        this.Person = user;
        this.ExternalLogins = await userManager.GetLoginsAsync(user);

        var changePasswordResult = await userManager.ChangePasswordAsync(user, this.Input.CurrentPassword, this.Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            this.Result = changePasswordResult;
            return this.Page();
        }

        await signInManager.RefreshSignInAsync(user);
        this.Result = IdentityResult.Success;
        logger.LogInformation("用户已成功更改其密码。");
        return this.Page();
    }
}
