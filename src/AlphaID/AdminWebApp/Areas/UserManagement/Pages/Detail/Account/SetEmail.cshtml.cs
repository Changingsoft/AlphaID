using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text;
using System.Web;
using AlphaIdPlatform;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Platform;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account
{
    public class SetEmailModel(ApplicationUserManager<NaturalPerson> userManager, IEmailSender emailSender, IOptions<SystemUrlInfo> systemUrlInfoOptions, IOptions<ProductInfo> productInfoOptions) : PageModel
    {

        [BindProperty]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        public string? Email { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var person = await userManager.FindByIdAsync(anchor);
            if (person == null)
            {
                return NotFound();
            }
            Email = person.Email;
            return Page();
        }

        public async Task<IActionResult> OnPost(string anchor)
        {
            var person = await userManager.FindByIdAsync(anchor);
            if (person == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await userManager.SetEmailAsync(person, Email);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(Email), error.Description);
                }
                return Page();
            }

            return RedirectToPage("Index", new { anchor });
        }
    }
}
