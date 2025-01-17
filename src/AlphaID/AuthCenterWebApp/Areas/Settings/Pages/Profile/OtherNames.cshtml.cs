using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Profile;

public class OtherNamesModel(UserManager<ApplicationUser> personManager, NaturalPersonService naturalPersonService) : PageModel
{
    [BindProperty]
    [Display(Name = "Nick name", Description = "2 to 10 characters. Leave blank to remove it.")]
    [StringLength(10, MinimumLength = 2, ErrorMessage = "Validate_StringLength")]
    public string? NickName { get; set; }

    public IdentityResult? Result { get; set; }

    public async Task OnGet()
    {
        ApplicationUser? person = await personManager.GetUserAsync(User);
        if (person == null)
            throw new InvalidOperationException("Can not find user.");

        NickName = person.NickName;
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        ApplicationUser? person = await personManager.GetUserAsync(User);
        if (person == null)
            throw new InvalidOperationException("Can not find user.");
        person.NickName = NickName;
        Result = await naturalPersonService.UpdateAsync(person);
        return Page();
    }
}