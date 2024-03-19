using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail
{
    public class PersonalInfoModel(NaturalPersonManager personManager) : PageModel
    {
        public NaturalPerson Person { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var person = await personManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            this.Person = person;
            this.Input = new InputModel()
            {
                DateOfBirth = this.Person.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
                Gender = this.Person.Gender,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var person = await personManager.FindByIdAsync(anchor);
            if (person == null)
                return this.NotFound();
            this.Person = person;

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Person.DateOfBirth = this.Input.DateOfBirth.HasValue ? DateOnly.FromDateTime(this.Input.DateOfBirth.Value) : null;
            this.Person.Gender = this.Input.Gender;

            this.Result = await personManager.UpdateAsync(this.Person);
            return this.Page();
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
}
