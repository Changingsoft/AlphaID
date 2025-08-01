using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class AdvancedModel(NaturalPersonService naturalPersonService, UserManager<NaturalPerson> applicationUserManager) : PageModel
{
    public NaturalPerson Data { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Data = person;

        Input = new InputModel
        {
            Enabled = Data.Enabled
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        NaturalPerson? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Data = person;

        if (!ModelState.IsValid)
            return Page();

        Data.Enabled = Input.Enabled;

        Result = await naturalPersonService.UpdateAsync(person);
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }
    }
}