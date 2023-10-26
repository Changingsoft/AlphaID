namespace AuthCenterWebApp.Pages.Grants;

public class ViewModel
{
    public IEnumerable<GrantViewModel> Grants { get; set; } = default!;
}

public class GrantViewModel
{
    public string ClientId { get; set; } = default!;
    public string ClientName { get; set; } = default!;
    public string? ClientUrl { get; set; }
    public string? ClientLogoUrl { get; set; }
    public string? Description { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Expires { get; set; }
    public IEnumerable<string> IdentityGrantNames { get; set; } = default!;
    public IEnumerable<string> ApiGrantNames { get; set; } = default!;
}