namespace AlphaIDWebAPITests.Controllers;
internal class PersonSearchResult
{
    public IEnumerable<PersonModel> Persons { get; set; } = default!;

    public bool More { get; set; }
}
