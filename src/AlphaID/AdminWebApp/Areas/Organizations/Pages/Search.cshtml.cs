using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages;

public class SearchModel(OrganizationSearcher searcher) : PageModel
{
    public IEnumerable<GenericOrganization> Results { get; set; } = default!;

    public IActionResult OnGet()
    {
        var q = Request.Query["q"];
        if (string.IsNullOrWhiteSpace(q))
            return Page();

        Results = searcher.Search(q!);
        return Page();
    }
}
