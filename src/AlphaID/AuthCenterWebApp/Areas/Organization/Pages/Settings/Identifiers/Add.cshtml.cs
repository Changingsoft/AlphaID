using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Identifiers;

public class AddModel(OrganizationManager organizationManager)
    : PageModel
{
    [BindProperty]
    [Display(Name = "Identifier Type")]
    public OrganizationIdentifierType Type { get; set; }

    [BindProperty]
    [Display(Name = "Identifier Value")]
    public string Value { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        var organization = organizationManager.FindByCurrentName(anchor);
        if (organization == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        var organization = organizationManager.FindByCurrentName(anchor);
        if (organization == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        var identifier = new OrganizationIdentifier
        {
            Type = Type, Value = Value
        };
        organization.OrganizationIdentifiers.Add(identifier);

        Result = await organizationManager.UpdateAsync(organization);
        if (Result.Succeeded)
            return RedirectToPage("Index", new { anchor });

        return Page();
    }
}