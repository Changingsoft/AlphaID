using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.UserManagement.Pages;

public class DeleteModel(UserManager<NaturalPerson> userManager, NaturalPersonService naturalPersonService) : PageModel
{
    public NaturalPerson Person { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
    [Required(ErrorMessage = "Validate_Required")]
    public string DisplayName { get; set; } = null!;


    public async Task<IActionResult> OnGetAsync(string id)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(id);
        if (person == null)
            return NotFound();
        Person = person;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        NaturalPerson? person = await userManager.FindByIdAsync(id);
        if (person == null)
            return NotFound();
        Person = person;


        if (DisplayName != Person.UserName)
            ModelState.AddModelError(nameof(DisplayName), "名称不一致");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            IdentityResult result = await naturalPersonService.DeleteAsync(Person);
            if (result.Succeeded)
            {
                return RedirectToPage("DeleteSuccess");
            }

            foreach (IdentityError error in result.Errors) ModelState.AddModelError("", error.Description);
            return Page();
        }
        catch (Exception ex)
        {
            ModelState.TryAddModelException("", ex);
            return Page();
        }
    }
}