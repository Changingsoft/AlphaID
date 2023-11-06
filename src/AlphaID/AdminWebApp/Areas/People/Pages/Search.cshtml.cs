using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages;

public class SearchModel : PageModel
{
    private readonly NaturalPersonManager personManager;

    public SearchModel(NaturalPersonManager personManager)
    {
        this.personManager = personManager;
    }

    public IEnumerable<NaturalPerson> Results { get; set; } = Array.Empty<NaturalPerson>();

    public async Task<IActionResult> OnGetAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return this.Page();

        if (MobilePhoneNumber.TryParse(q, out MobilePhoneNumber mobile))
        {
            var person = await this.personManager.FindByMobileAsync(mobile.ToString());
            return person != null ? this.RedirectToPage("Detail/Index", new { id = person.Id }) : this.Page();
        }

        var pinyinResult = new List<NaturalPerson>(this.personManager.Users.Where(p => p.PhoneticSearchHint!.StartsWith(q)).OrderBy(p => p.PhoneticSearchHint!.Length).ThenBy(p => p.PhoneticSearchHint));
        var nameResult = new List<NaturalPerson>(this.personManager.Users.Where(p => p.Name.StartsWith(q)).OrderBy(p => p.Name.Length).ThenBy(p => p.Name));

        this.Results = pinyinResult.UnionBy(nameResult, p => p.Id);
        return this.Page();
    }
}
