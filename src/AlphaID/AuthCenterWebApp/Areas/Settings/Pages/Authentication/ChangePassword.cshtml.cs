using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        Person = user;
        ExternalLogins = await userManager.GetLoginsAsync(user);
        bool hasPassword = await userManager.HasPasswordAsync(user);
        return !hasPassword ? RedirectToPage("./SetPassword") : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        NaturalPerson? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
        Person = user;
        ExternalLogins = await userManager.GetLoginsAsync(user);

        IdentityResult changePasswordResult =
            await userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
        if (!changePasswordResult.Succeeded)
        {
            Result = changePasswordResult;
            return Page();
        }

        await signInManager.RefreshSignInAsync(user);
        Result = IdentityResult.Success;
        logger.LogInformation("用户已成功更改其密码。");
        return Page();
    }

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
}