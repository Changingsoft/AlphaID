using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class DeleteModel(NaturalPersonManager userManager) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    public NaturalPerson Person { get; set; } = default!;

    [BindProperty]
    public DeletePersonForm Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await userManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await userManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;


        if (Input.DisplayName != Person.PersonName.FullName)
        {
            ModelState.AddModelError(nameof(Input.DisplayName), "Ãû³Æ²»Ò»ÖÂ");
        }

        if (!ModelState.IsValid)
            return Page();

        try
        {
            var result = await userManager.DeleteAsync(Person);
            if (result.Succeeded)
                return RedirectToPage("DeleteSuccess");
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Page();
            }
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
        public string DisplayName { get; set; } = default!;
    }
}
