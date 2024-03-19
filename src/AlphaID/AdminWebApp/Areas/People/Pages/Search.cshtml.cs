using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages;

public class SearchModel(NaturalPersonManager personManager) : PageModel
{
    public IEnumerable<NaturalPerson> Results { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return this.Page();

        if (MobilePhoneNumber.TryParse(q, out MobilePhoneNumber mobile))
        {
            var person = await personManager.FindByMobileAsync(mobile.ToString(), this.HttpContext.RequestAborted);
            return person != null ? this.RedirectToPage("Detail/Index", new { id = person.Id }) : this.Page();
        }

        var pinyinResult = new List<NaturalPerson>(personManager.Users.Where(p => p.PersonName.SearchHint!.StartsWith(q)).OrderBy(p => p.PersonName.SearchHint!.Length).ThenBy(p => p.PersonName.SearchHint));
        var nameResult = new List<NaturalPerson>(personManager.Users.Where(p => p.PersonName.FullName.StartsWith(q)).OrderBy(p => p.PersonName.FullName.Length).ThenBy(p => p.PersonName.FullName));

        this.Results = pinyinResult.UnionBy(nameResult, p => p.Id);
        return this.Page();
    }
}
