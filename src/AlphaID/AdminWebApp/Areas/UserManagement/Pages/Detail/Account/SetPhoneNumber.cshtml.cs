using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account
{
    public class SetPhoneNumberModel(UserManager<NaturalPerson> userManager, NaturalPersonService personService) : PageModel
    {

        [BindProperty]
        [Display(Name = "Phone number", Description = "留空则删除此移动电话号码。")]
        [StringLength(16, MinimumLength = 11, ErrorMessage = "Validate_StringLength")]
        public string? PhoneNumber { get; set; }

        [BindProperty]
        [Display(Name = "Confirm this phone number")]
        public bool PhoneNumberConfirmed { get; set; }

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

            if (!string.IsNullOrEmpty(PhoneNumber) && !MobilePhoneNumber.TryParse(PhoneNumber, out _))
            {
                ModelState.AddModelError(nameof(PhoneNumber), "Invalid phone number.");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await personService.SetPhoneNumberAsync(person, PhoneNumber, PhoneNumberConfirmed);
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
