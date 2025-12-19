using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Identifiers;

public class IndexModel(OrganizationManager organizationManager)
    : PageModel
{
    public IEnumerable<OrganizationIdentifier> Identifiers { get; set; } = [];

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Identifiers = organization.OrganizationIdentifiers;
        return Page();
    }

    public async Task<IActionResult> OnPostRemove(string anchor, string idKey)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Identifiers = organization.OrganizationIdentifiers;

        string[] keyPart = idKey.Split('|');
        var type = Enum.Parse<OrganizationIdentifierType>(keyPart[0]);
        OrganizationIdentifier? identifier = Identifiers.FirstOrDefault(i => i.Type == type && i.Value == keyPart[1]);
        if (identifier == null)
            return Page();

        organization.OrganizationIdentifiers.Remove(identifier);

        Result = await organizationManager.UpdateAsync(organization);
        return Page();
    }
}