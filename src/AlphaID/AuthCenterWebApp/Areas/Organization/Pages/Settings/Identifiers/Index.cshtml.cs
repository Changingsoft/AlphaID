using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Identifiers;

public class IndexModel(OrganizationManager organizationManager, OrganizationIdentifierManager identifierManager)
    : PageModel
{
    public IEnumerable<OrganizationIdentifier> Identifiers { get; set; } = [];

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();
        Identifiers = identifierManager.GetIdentifiers(organization);
        return Page();
    }

    public async Task<IActionResult> OnPostRemove(string anchor, string idKey)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();
        Identifiers = identifierManager.GetIdentifiers(organization);

        string[] keyPart = idKey.Split('|');
        var type = Enum.Parse<OrganizationIdentifierType>(keyPart[0]);
        OrganizationIdentifier? identifier = Identifiers.FirstOrDefault(i => i.Type == type && i.Value == keyPart[1]);
        if (identifier == null)
            return Page();

        Result = await identifierManager.RemoveIdentifierAsync(identifier);
        if (Result.Succeeded) Identifiers = identifierManager.GetIdentifiers(organization);

        return Page();
    }
}