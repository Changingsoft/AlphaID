using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class DeleteModel(UserManager<NaturalPerson> userManager, NaturalPersonService naturalPersonService) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = null!;

    public NaturalPerson Person { get; set; } = null!;

    [BindProperty]
    public DeletePersonForm Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await userManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? person = await userManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;


        if (Input.DisplayName != Person.UserName)
            ModelState.AddModelError(nameof(Input.DisplayName), "名称不一致");

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

    public class DeletePersonForm
    {
        [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
        [Required(ErrorMessage = "Validate_Required")]
        public string DisplayName { get; set; } = null!;
    }
}