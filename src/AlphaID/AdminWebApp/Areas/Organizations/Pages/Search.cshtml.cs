using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages;

public class SearchModel : PageModel
{
    private readonly OrganizationSearcher searcher;

    public SearchModel(OrganizationSearcher searcher)
    {
        this.searcher = searcher;
    }

    public IEnumerable<GenericOrganization> Results { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var q = this.Request.Query["q"];
        if (string.IsNullOrWhiteSpace(q))
            return this.Page();

        this.Results = this.searcher.Search(q!);
        return this.Page();
    }
}
