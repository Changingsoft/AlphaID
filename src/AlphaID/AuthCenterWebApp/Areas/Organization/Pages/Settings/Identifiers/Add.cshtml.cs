using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Identifiers;

public class AddModel(OrganizationIdentifierManager identifierManager, OrganizationManager organizationManager)
    : PageModel
{
    [BindProperty]
    [Display(Name = "Identifier Type")]
    public OrganizationIdentifierType Type { get; set; }

    [BindProperty]
    [Display(Name = "Identifier Value")]
    public string Value { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var identifier = new OrganizationIdentifier
        {
            Organization = organization, OrganizationId = organization.Id, Type = Type, Value = Value
        };

        Result = await identifierManager.AddIdentifierAsync(identifier);
        if (Result.Succeeded)
            return RedirectToPage("Index", new { anchor });

        return Page();
    }
}