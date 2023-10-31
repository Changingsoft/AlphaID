using IDSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ResetPasswordModel : PageModel
{
    private readonly NaturalPersonManager _userManager;

    public ResetPasswordModel(NaturalPersonManager userManager)
    {
        this._userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IActionResult OnGet(string code)
    {
        if (code == null)
        {
            return this.BadRequest("A code must be supplied for password reset.");
        }
        else
        {
            this.Input = new InputModel
            {
                Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
            };
            return this.Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = await this._userManager.FindByEmailAsync(this.Input.Email);
        if (user == null)
        {
            // Don't reveal that the user does not exist
            return this.RedirectToPage("./ResetPasswordConfirmation");
        }

        var result = await this._userManager.ResetPasswordAsync(user, this.Input.Code, this.Input.Password);
        if (result.Succeeded)
        {
            return this.RedirectToPage("./ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            this.ModelState.AddModelError(string.Empty, error.Description);
        }
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Validate_Required")]
        [EmailAddress(ErrorMessage = "{0}µÄ¸ñÊ½´íÎó")]
        public string Email { get; set; } = default!;

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
        public string Password { get; set; } = default!;

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
        [Compare("Password", ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
        public string Code { get; set; } = default!;

    }
}
