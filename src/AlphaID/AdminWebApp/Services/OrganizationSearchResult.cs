namespace AdminWebApp.Services;

public class OrganizationSearchResult
{
    public IEnumerable<OrganizationModel> Organizations { get; set; } = default!;

    public bool More { get; set; }
}