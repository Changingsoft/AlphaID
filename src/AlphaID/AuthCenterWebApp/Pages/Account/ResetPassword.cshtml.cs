using System.ComponentModel.DataAnnotations;
using System.Text;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AuthCenterWebApp.Pages.Account;

[SecurityHeaders]
[AllowAnonymous]
public class ResetPasswordModel(NaturalPersonService naturalPersonService, NaturalPersonManager userManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IActionResult OnGet(string? code)
    {
        if (code == null) return BadRequest("A code must be supplied for password reset.");

        Input = new InputModel
        {
            Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        NaturalPerson? user = await userManager.FindByEmailAsync(Input.Email);
        if (user == null)
            // Don't reveal that the user does not exist
            return RedirectToPage("./ResetPasswordConfirmation");

        IdentityResult result = await naturalPersonService.ResetPasswordAsync(user, Input.Code, Input.Password);
        if (result.Succeeded) return RedirectToPage("./ResetPasswordConfirmation");

        foreach (IdentityError error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Validate_Required")]
        [EmailAddress(ErrorMessage = "{0}的格式错误")]
        public string Email { get; set; } = null!;

        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
        public string Password { get; set; } = null!;

        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 8, ErrorMessage = "Validate_StringLength")]
        [Compare("Password", ErrorMessage = "Validate_PasswordConfirm")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        public string Code { get; set; } = null!;
    }
}