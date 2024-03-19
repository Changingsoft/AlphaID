using IdSubjects;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Settings.Pages.Profile
{
    public class PersonNameModel(NaturalPersonManager personManager, RealNameManager realNameManager) : PageModel
    {
        [BindProperty]
        public InputMode Input { get; set; } = default!;

        public IdentityResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var person = await personManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.NotFound();
            }

            var hasRealName = realNameManager.GetAuthentications(person).Any();
            if (hasRealName)
            {
                this.Result = IdentityResult.Failed(new IdentityError() { Code = "Cannot change name after real-name authentication", Description = "You cannot change name because your has been passed real-name authentication." });
            }

            this.Input = new InputMode()
            {
                Surname = person.PersonName.Surname,
                MiddleName = person.PersonName.MiddleName,
                GivenName = person.PersonName.GivenName,
                PhoneticSurname = person.PhoneticSurname,
                PhoneticGivenName = person.PhoneticGivenName,
            };
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var person = await personManager.GetUserAsync(this.User);
            if (person == null)
            {
                return this.NotFound();
            }

            person.PersonName = new PersonNameInfo($"{this.Input.Surname}{this.Input.GivenName}", this.Input.Surname, this.Input.GivenName, this.Input.MiddleName);
            person.PhoneticSurname = this.Input.PhoneticSurname;
            person.PhoneticGivenName = this.Input.PhoneticGivenName;

            this.Result = await personManager.UpdateAsync(person);
            return this.Page();
        }

        public class InputMode
        {
            [Display(Name = "Surname")]
            [StringLength(10, ErrorMessage = "Validate_StringLength")]
            public string? Surname { get; set; }

            [Display(Name = "Middle name")]
            [StringLength(10, ErrorMessage = "Validate_StringLength")]
            public string? MiddleName { get; set; }

            [Display(Name = "Given name")]
            [StringLength(10, ErrorMessage = "Validate_StringLength")]
            public string? GivenName { get; set; }

            [Display(Name = "Phonetic surname")]
            [StringLength(10, ErrorMessage = "Validate_StringLength")]
            public string? PhoneticSurname { get; set; }

            [Display(Name = "Phonetic given name")]
            [StringLength(10, ErrorMessage = "Validate_StringLength")]
            public string? PhoneticGivenName { get; set; }
        }
    }
}
