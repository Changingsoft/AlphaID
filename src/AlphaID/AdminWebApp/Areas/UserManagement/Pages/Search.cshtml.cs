using IdSubjects;
using IdSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages;

public class SearchModel(ApplicationUserManager personManager) : PageModel
{
    public IEnumerable<ApplicationUser> Results { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Page();

        if (MobilePhoneNumber.TryParse(q, out MobilePhoneNumber mobile))
        {
            ApplicationUser? person =
                await personManager.FindByMobileAsync(mobile.ToString(), HttpContext.RequestAborted);
            return person != null ? RedirectToPage("Detail/Index", new { id = person.Id }) : Page();
        }

        var pinyinResult = new List<ApplicationUser>(personManager.Users
            .Where(p => p.PersonName.SearchHint!.StartsWith(q)).OrderBy(p => p.PersonName.SearchHint!.Length)
            .ThenBy(p => p.PersonName.SearchHint));
        var nameResult = new List<ApplicationUser>(personManager.Users.Where(p => p.PersonName.FullName.StartsWith(q))
            .OrderBy(p => p.PersonName.FullName.Length).ThenBy(p => p.PersonName.FullName));

        Results = pinyinResult.UnionBy(nameResult, p => p.Id);
        return Page();
    }
}