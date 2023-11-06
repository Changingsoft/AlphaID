using IDSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Profile
{
    public class PersonNameModel : PageModel
    {
        private readonly NaturalPersonManager personManager;

        public PersonNameModel(NaturalPersonManager personManager)
        {
            this.personManager = personManager;
        }

        public InputMode Input { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var person = await this.personManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.NotFound();
            }
            this.Input = new InputMode()
            {
                Surname = person.LastName,
                MiddleName = person.MiddleName,
                GivenName = person.FirstName,
                PhoneticSurname = person.PhoneticSurname,
                PhoneticGivenName = person.PhoneticGivenName,
                NickName = person.NickName,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var person = await this.personManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.NotFound();
            }
            person.LastName = this.Input.Surname;
            person.MiddleName = this.Input.MiddleName;
            person.FirstName = this.Input.GivenName;
            person.PhoneticSurname = this.Input.PhoneticSurname;
            person.PhoneticGivenName = this.Input.PhoneticGivenName;
            person.NickName = this.Input.NickName;

            this.Result = await this.personManager.UpdateAsync(person);
            return this.Page();
        }

        public class InputMode
        {
            [Display(Name = "Surname")]
            [StringLength(10)]
            public string? Surname { get; init; }

            [Display(Name = "MiddleName")]
            [StringLength(10)]
            public string? MiddleName { get; init; }

            [Display(Name = "GivenName")]
            [StringLength(10)]
            public string? GivenName { get; init; }

            [Display(Name = "PhoneticSurname")]
            [StringLength(10)]
            public string? PhoneticSurname { get; init; }

            [Display(Name = "PhoneticGivenName")]
            [StringLength(10)]
            public string? PhoneticGivenName { get; init; }

            [Display(Name = "NickName")]
            [StringLength(10)]
            public string? NickName { get; init; }
        }
    }
}
