#nullable disable

using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Authentication;

public class SetPasswordModel(
    NaturalPersonService naturalPersonService,
    ApplicationUserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        bool hasPassword = await userManager.HasPasswordAsync(user);

        return hasPassword ? RedirectToPage("./ChangePassword") : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        ApplicationUser user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        IdentityResult addPasswordResult = await naturalPersonService.AddPasswordAsync(user, Input.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            foreach (IdentityError error in addPasswordResult.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await signInManager.RefreshSignInAsync(user);
        StatusMessage = "您的密码已设置。";

        return RedirectToPage();
    }

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
}