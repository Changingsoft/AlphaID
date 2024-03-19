using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages;

public class SearchModel(OrganizationSearcher searcher) : PageModel
{
    public IEnumerable<GenericOrganization> Results { get; set; } = default!;

    public IActionResult OnGet()
    {
        var q = this.Request.Query["q"];
        if (string.IsNullOrWhiteSpace(q))
            return this.Page();

        this.Results = searcher.Search(q!);
        return this.Page();
    }
}
