using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Pages.Consent;

public class InputModel
{
    public string Button { get; set; } = default!;
    public IEnumerable<string> ScopesConsented { get; set; } = default!;

    [Display(Name = "Remember my decision")]
    public bool RememberConsent { get; set; } = true;

    public string ReturnUrl { get; set; } = default!;

    [Display(Name = "Description")]
    public string? Description { get; set; }
}