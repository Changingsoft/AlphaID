using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class EditPersonNameModel : PageModel
{
    private readonly NaturalPersonManager naturalPersonManager;

    public EditPersonNameModel(NaturalPersonManager naturalPersonManager)
    {
        this.naturalPersonManager = naturalPersonManager;
    }

    public InputModel Input { get; set; } = default!;

    public async Task<IActionResult> OnGet(string id)
    {
        var person = await this.naturalPersonManager.FindByIdAsync(id);
        if (person == null)
        {
            return this.NotFound();
        }
        this.Input = new()
        {
            Surname = person.LastName,
            GivenName = person.FirstName ?? default!,
            PinyinSurname = person.PhoneticSurname,
            PinyinGivenName = person.PhoneticGivenName ?? default!,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        var person = await this.naturalPersonManager.FindByIdAsync(id);
        if (person == null) { return this.NotFound(); }

        if (!this.ModelState.IsValid)
            return this.Page();

        var chinesePersonName = new ChinesePersonName(this.Input.Surname, this.Input.GivenName, this.Input.PinyinSurname, this.Input.PinyinGivenName);
        await this.naturalPersonManager.ForceChangeNameAsync(person, chinesePersonName);
        return this.RedirectToPage("Index");
    }

    public class InputModel
    {
        [Display(Name = "–’ œ")]
        [StringLength(10)]
        public string? Surname { get; set; } = default!;

        [Display(Name = "√˚◊÷")]
        [StringLength(10)]
        public string GivenName { get; set; } = default!;

        [Display(Name = "–’ œ∆¥“Ù")]
        [StringLength(30)]
        public string? PinyinSurname { get; set; } = default!;

        [Display(Name = "√˚◊÷∆¥“Ù")]
        [StringLength(30)]
        public string PinyinGivenName { get; set; } = default!;
    }
}
