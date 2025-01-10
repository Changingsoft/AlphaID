using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class PersonalInfoModel(NaturalPersonManager personManager) : PageModel
{
    public NaturalPerson Person { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;
        Input = new InputModel
        {
            DateOfBirth = Person.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
            Gender = Person.Gender
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(anchor);
        if (person == null)
            return NotFound();
        Person = person;

        if (!ModelState.IsValid)
            return Page();

        Person.DateOfBirth = Input.DateOfBirth.HasValue ? DateOnly.FromDateTime(Input.DateOfBirth.Value) : null;
        Person.Gender = Input.Gender;

        Result = await personManager.UpdateAsync(Person);
        return Page();
    }

    public record InputModel
    {
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }
    }
}