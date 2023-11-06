#nullable disable

using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Account;

public class DeletePersonalDataModel : PageModel
{
    private readonly NaturalPersonManager userManager;
    private readonly SignInManager<NaturalPerson> signInManager;
    private readonly ILogger<DeletePersonalDataModel> logger;

    public DeletePersonalDataModel(
        NaturalPersonManager userManager,
        SignInManager<NaturalPerson> signInManager,
        ILogger<DeletePersonalDataModel> logger)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; init; }
    }

    public bool RequirePassword { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        this.RequirePassword = await this.userManager.HasPasswordAsync(user);
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await this.userManager.GetUserAsync(this.User);
        if (user == null)
        {
            return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
        }

        this.RequirePassword = await this.userManager.HasPasswordAsync(user);
        if (this.RequirePassword)
        {
            if (!await this.userManager.CheckPasswordAsync(user, this.Input.Password))
            {
                this.ModelState.AddModelError(string.Empty, "Incorrect password.");
                return this.Page();
            }
        }

        var result = await this.userManager.DeleteAsync(user);
        var userId = await this.userManager.GetUserIdAsync(user);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Unexpected error occurred deleting user.");
        }

        await this.signInManager.SignOutAsync();

        this.logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

        return this.Redirect("~/");
    }
}
