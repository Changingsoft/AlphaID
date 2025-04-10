using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account
{
    public class SetPhoneNumberModel(ApplicationUserManager<NaturalPerson> userManager) : PageModel
    {

        [BindProperty]
        [Display(Name = "Phone number")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Validate_StringLength")]
        public string? PhoneNumber { get; set; }

        public async Task<IActionResult> OnGet(string anchor)
        {
            var person = await userManager.FindByIdAsync(anchor);
            if (person == null)
            {
                return NotFound();
            }

            if (person.PhoneNumber != null && MobilePhoneNumber.TryParse(person.PhoneNumber, out var number))
            {
                PhoneNumber = number.PhoneNumber;
            }
            return Page();
        }

        public async Task<IActionResult> OnPost(string anchor)
        {
            var person = await userManager.FindByIdAsync(anchor);
            if (person == null)
            {
                return NotFound();
            }

            if (!MobilePhoneNumber.TryParse(PhoneNumber, out var number))
            {
                ModelState.AddModelError(nameof(PhoneNumber), "Invalid phone number.");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await userManager.SetPhoneNumberAsync(person, number.ToString());
            if (result.Succeeded)
            {
                return RedirectToPage("Index", new { anchor });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(PhoneNumber), error.Description);
            }
            return Page();
        }
    }
}
