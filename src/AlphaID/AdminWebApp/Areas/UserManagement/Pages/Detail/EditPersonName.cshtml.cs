using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using IdSubjects;
using IdSubjects.ChineseName;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class EditPersonNameModel(ApplicationUserManager<NaturalPerson> applicationUserManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        NaturalPerson? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();
        Input = new InputModel
        {
            Surname = person.FamilyName,
            GivenName = person.GivenName ?? null!,
            PinyinSurname = person.PhoneticSurname,
            PinyinGivenName = person.PhoneticGivenName ?? null!
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        NaturalPerson? person = await applicationUserManager.FindByIdAsync(anchor);
        if (person == null) return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var chinesePersonName =
            new ChinesePersonName(Input.Surname, Input.GivenName, Input.PinyinSurname, Input.PinyinGivenName);
        person.FamilyName = chinesePersonName.Surname;
        person.GivenName = chinesePersonName.GivenName;
        person.Name = chinesePersonName.FullName;
        person.PhoneticSurname = chinesePersonName.PhoneticSurname;
        person.PhoneticGivenName = chinesePersonName.PhoneticGivenName;
        person.SearchHint = chinesePersonName.PhoneticName;
        await applicationUserManager.UpdateAsync(person);
        return RedirectToPage("Index", new { anchor });
    }

    public class InputModel
    {
        [Display(Name = "Surname")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string? Surname { get; set; }

        [Display(Name = "Given name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(10, ErrorMessage = "Validate_StringLength")]
        public string GivenName { get; set; } = null!;

        [Display(Name = "Phonetic surname")]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        public string? PinyinSurname { get; set; }

        [Display(Name = "Phonetic given name")]
        [Required(ErrorMessage = "Validate_Required")]
        [StringLength(30, ErrorMessage = "Validate_StringLength")]
        public string? PinyinGivenName { get; set; }
    }
}