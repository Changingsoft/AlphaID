namespace AuthCenterWebApp.Pages.Ciba;

public class ScopeViewModel
{
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }
    public bool Emphasize { get; set; }
    public bool Required { get; set; }
    public bool Checked { get; set; }
    public IEnumerable<ResourceViewModel> Resources { get; set; } = null!;
}