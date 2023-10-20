namespace AlphaIDWebAPITests.Models;
internal record OrganizationSearchResult(bool More)
{
    public IEnumerable<OrganizationModel> Organizations { get; set; } = default!;
}
