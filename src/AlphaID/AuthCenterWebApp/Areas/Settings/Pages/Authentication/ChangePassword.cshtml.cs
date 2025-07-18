using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class ChangePasswordModel(
    NaturalPersonService naturalPersonService,
    UserManager<NaturalPerson> userManager,
    SignInManager<NaturalPerson> signInManager,
    ILogger<ChangePasswordModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public NaturalPerson Person { get; set; } = null!;

    public IList<UserLoginInfo> ExternalLogins { get; set; } = null!;

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
            await naturalPersonService.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
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
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(100, ErrorMessage = "Validate_StringLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; } = null!;
    }
}