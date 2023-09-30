namespace AdminWebApp.Services;

public class PersonSearchResult
{
    public ICollection<NaturalPersonModel> Persons { get; set; } = default!;

    public bool More { get; set; }
}