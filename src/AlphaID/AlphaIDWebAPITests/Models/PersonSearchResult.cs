namespace AlphaIDWebAPITests.Models;

internal record PersonSearchResult(bool More)
{
    public IEnumerable<PersonModel> Persons { get; set; } = default!;
}
