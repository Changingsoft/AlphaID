using System.ComponentModel.DataAnnotations;
using IdSubjects;
using IdSubjects.ChineseName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class EditPersonNameModel(NaturalPersonManager naturalPersonManager) : PageModel
{
    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await naturalPersonManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();
        Input = new InputModel
        {
            Surname = person.PersonName.Surname,
            GivenName = person.PersonName.GivenName ?? default!,
            PinyinSurname = person.PhoneticSurname,
            PinyinGivenName = person.PhoneticGivenName ?? default!
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        NaturalPerson? person = await naturalPersonManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var chinesePersonName =
            new ChinesePersonName(Input.Surname, Input.GivenName, Input.PinyinSurname, Input.PinyinGivenName);
        var personName = new PersonNameInfo(chinesePersonName.FullName, chinesePersonName.Surname,
            chinesePersonName.GivenName);
        await naturalPersonManager.AdminChangePersonNameAsync(person, personName);
        return RedirectToPage("Index");
    }

    public class InputModel
    {
        [Display(Name = "Surname")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string? Surname { get; set; }

        [Display(Name = "Given name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string GivenName { get; set; } = default!;

        [Display(Name = "Phonetic surname")]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        public string? PinyinSurname { get; set; }

        [Display(Name = "Phonetic given name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        public string PinyinGivenName { get; set; } = default!;
    }
}