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
        var person = await userManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await userManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;


        if (this.Input.DisplayName != this.Person.PersonName.FullName)
        {
            this.ModelState.AddModelError(nameof(this.Input.DisplayName), "Ãû³Æ²»Ò»ÖÂ");
        }

        if (!this.ModelState.IsValid)
            return this.Page();

        try
        {
            var result = await userManager.DeleteAsync(this.Person);
            if (result.Succeeded)
                return this.RedirectToPage("DeleteSuccess");
            else
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError("", error.Description);
                }
                return this.Page();
            }
        }
        catch (Exception ex)
        {
            this.ModelState.TryAddModelException("", ex);
            return this.Page();
        }
    }

    public class DeletePersonForm
    {

        [Display(Name = "Display name", Description = "A friendly name that appears on the user interface.")]
        [Required(ErrorMessage = "Validate_Required")]
        public string DisplayName { get; set; } = default!;
    }
}
