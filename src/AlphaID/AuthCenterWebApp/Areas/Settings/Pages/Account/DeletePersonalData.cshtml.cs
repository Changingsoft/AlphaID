using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class DeletePersonalDataModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<DeletePersonalDataModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        RequirePassword = await userManager.HasPasswordAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ApplicationUser? user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");

        RequirePassword = await userManager.HasPasswordAsync(user);
        if (RequirePassword)
            if (!await userManager.CheckPasswordAsync(user, Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Incorrect password.");
                return Page();
            }

        IdentityResult result = await userManager.DeleteAsync(user);
        string userId = await userManager.GetUserIdAsync(user);
        if (!result.Succeeded) throw new InvalidOperationException("Unexpected error occurred deleting user.");

        await signInManager.SignOutAsync();

        logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

        return Redirect("~/");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;
    }
}