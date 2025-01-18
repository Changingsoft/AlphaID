using AlphaIdPlatform.Subjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace AdminWebApp.Areas.OrganizationManagement.Pages;

public class SearchModel(OrganizationSearcher searcher) : PageModel
{
    public IEnumerable<Organization> Results { get; set; } = [];

    public IActionResult OnGet()
    {
        StringValues q = Request.Query["q"];
        if (string.IsNullOrWhiteSpace(q))
            return Page();

        Results = searcher.Search(q!);
        return Page();
    }
}