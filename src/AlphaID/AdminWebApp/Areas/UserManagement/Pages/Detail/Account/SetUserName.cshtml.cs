using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account
{
    public class SetUserNameModel(UserManager<NaturalPerson> userManager) : PageModel
    {

        [BindProperty]
        [Display(Name = "User name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Validate_StringLength")]
        public string UserName { get; set; } = null!;

        public async Task<IActionResult> OnGet(string anchor)
        {
            var person = await userManager.FindByIdAsync(anchor);
            if (person == null)
            {
                return NotFound();
            }
            UserName = person.UserName!;
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
            var result = await userManager.SetUserNameAsync(person, UserName);
            if (result.Succeeded)
            {
                return RedirectToPage("Index", new { anchor });
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(nameof(UserName), error.Description);
            }
            return Page();
        }
    }
}
