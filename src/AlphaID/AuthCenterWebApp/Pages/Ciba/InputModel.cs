namespace AuthCenterWebApp.Pages.Ciba;

public class InputModel
{
    public string Button { get; set; } = default!;
    public IEnumerable<string> ScopesConsented { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string Description { get; set; } = default!;
}