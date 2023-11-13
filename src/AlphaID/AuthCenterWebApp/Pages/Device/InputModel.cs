namespace AuthCenterWebApp.Pages.Device;

public class InputModel
{
    public string Button { get; set; } = default!;
    public IEnumerable<string>? ScopesConsented { get; set; }
    public bool RememberConsent { get; set; } = true;
    public string ReturnUrl { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string UserCode { get; set; } = default!;
}