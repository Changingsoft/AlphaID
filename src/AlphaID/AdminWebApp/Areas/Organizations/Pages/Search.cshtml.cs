using IDSubjects;
using IDSubjects.Subjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages;

public class SearchModel : PageModel
{
    private readonly IOrganizationStore organizationStore;
    private readonly OrganizationSearcher searcher;

    public SearchModel(IOrganizationStore organizationStore, OrganizationSearcher searcher)
    {
        this.organizationStore = organizationStore;
        this.searcher = searcher;
    }

    public IEnumerable<GenericOrganization> Results { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var q = this.Request.Query["q"];
        if (string.IsNullOrWhiteSpace(q))
            return this.Page();

        if (Uscc.TryParse(q!, out Uscc number))
        {
            var org = this.organizationStore.Organizations.FirstOrDefault(p => p.Usci == number.ToString());
            return org != null ? this.RedirectToPage("Detail/Index", new { id = org.Id }) : this.Page();
        }

        this.Results = this.searcher.Search(q!);
        return this.Page();
    }
}
