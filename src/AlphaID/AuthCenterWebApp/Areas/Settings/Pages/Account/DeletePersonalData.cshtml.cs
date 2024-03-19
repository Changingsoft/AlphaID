#nullable disable

using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class DeletePersonalDataModel(
    NaturalPersonManager userManager,
    SignInManager<NaturalPerson> signInManager,
    ILogger<DeletePersonalDataModel> logger) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }

        this.RequirePassword = await userManager.HasPasswordAsync(user);
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{userManager.GetUserId(this.User)}'.");
        }

        this.RequirePassword = await userManager.HasPasswordAsync(user);
        if (this.RequirePassword)
        {
            if (!await userManager.CheckPasswordAsync(user, this.Input.Password))
            {
                this.ModelState.AddModelError(string.Empty, "Incorrect password.");
                return this.Page();
            }
        }

        var result = await userManager.DeleteAsync(user);
        var userId = await userManager.GetUserIdAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Unexpected error occurred deleting user.");
        }

        await signInManager.SignOutAsync();

        logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

        return this.Redirect("~/");
    }
}
