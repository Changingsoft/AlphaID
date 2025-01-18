using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects.RealName;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Settings.Pages.Profile;

public class PersonNameModel(UserManager<NaturalPerson> personManager, RealNameManager<NaturalPerson> realNameManager, NaturalPersonService naturalPersonService) : PageModel
{
    [BindProperty]
    public InputMode Input { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        if (person == null) return NotFound();

        bool hasRealName = realNameManager.GetAuthentications(person).Any();
        if (hasRealName)
            Result = IdentityResult.Failed(new IdentityError
            {
                Code = "Cannot change name after real-name authentication",
                Description = "You cannot change name because your has been passed real-name authentication."
            });

        Input = new InputMode
        {
            Surname = person.FamilyName,
            MiddleName = person.MiddleName,
            GivenName = person.GivenName,
            PhoneticSurname = person.PhoneticSurname,
            PhoneticGivenName = person.PhoneticGivenName
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? person = await personManager.GetUserAsync(User);
        if (person == null) return NotFound();

        person.FamilyName = Input.Surname;
        person.GivenName = Input.GivenName;
        person.Name = $"{Input.Surname}{Input.GivenName}";
        person.PhoneticSurname = Input.PhoneticSurname;
        person.PhoneticGivenName = Input.PhoneticGivenName;

        Result = await naturalPersonService.UpdateAsync(person);
        return Page();
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