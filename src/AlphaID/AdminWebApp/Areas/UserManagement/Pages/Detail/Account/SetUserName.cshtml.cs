using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account
{
    public class SetUserNameModel(NaturalPersonService personService, ApplicationUserManager<NaturalPerson> userManager) : PageModel
    {

        [BindProperty]
        [Display(Name = "User Name")]
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
            person.UserName = UserName;
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
