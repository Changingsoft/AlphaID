using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using IdSubjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthCenterWebApp.Pages.Account
{
    [AllowAnonymous]
    public class VerifyPhoneNumberModel(ApplicationUserManager<NaturalPerson> userManager, IVerificationCodeService verificationCodeService) : PageModel
    {
        [BindProperty]
        [Display(Name = "Verification code")]
        [Required(ErrorMessage = "Validate_Required")]
        [MaxLength(6, ErrorMessage = "Validate_MaxLength")]
        public string VerificatonCode { get;set; } = null!;

        public IActionResult OnGet()
        {
            var resetPasswordPhoneNumber = HttpContext.Session.GetString("ResetPasswordPhoneNumber");
            if(resetPasswordPhoneNumber == null)
            {
                throw new InvalidOperationException("ResetPasswordPhoneNumber is not found in session.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var resetPasswordPhoneNumber = HttpContext.Session.GetString("ResetPasswordPhoneNumber");
            if (resetPasswordPhoneNumber == null)
            {
                throw new InvalidOperationException("ResetPasswordPhoneNumber is not found in session.");
            }

            if(await verificationCodeService.VerifyAsync(resetPasswordPhoneNumber, VerificatonCode))
            {
                //Generate reset password token
                var person = await userManager.FindByMobileAsync(resetPasswordPhoneNumber);
                if(person == null)
                    throw new InvalidOperationException("Person is not found by mobile number.");

                var code = await userManager.GeneratePasswordResetTokenAsync(person);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                //Clear session data
                HttpContext.Session.Remove("ResetPasswordPhoneNumber");

                return RedirectToPage("ResetPasswordMobile", new { phone = resetPasswordPhoneNumber, code });
            }
            else
            {
                ModelState.AddModelError(nameof(VerificatonCode), "Invalid verification code");
                return Page();
            }
        }
    }
}
