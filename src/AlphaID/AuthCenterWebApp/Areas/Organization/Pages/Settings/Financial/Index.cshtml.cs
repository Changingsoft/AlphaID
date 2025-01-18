using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class IndexModel(OrganizationManager organizationManager) : PageModel
{
    public AlphaIdPlatform.Subjects.Organization Data { get; set; } = null!;

    public ICollection<OrganizationBankAccount> BankAccounts { get; set; } = [];

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();
        Data = organization;
        BankAccounts = organization.BankAccounts;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string accountNumber)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();
        Data = organization;
        BankAccounts = organization.BankAccounts;

        OrganizationBankAccount? bankAccount = BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null) return Page();

        BankAccounts.Remove(bankAccount);

        Result = await organizationManager.UpdateAsync(organization);
        return Page();
    }

    public async Task<IActionResult> OnPostSetDefaultAsync(string anchor, string accountNumber)
    {
        AlphaIdPlatform.Subjects.Organization? data = await organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();
        Data = data;
        BankAccounts = data.BankAccounts;

        OrganizationBankAccount? bankAccount = BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null) return Page();

        var oldAccount = BankAccounts.FirstOrDefault(b => b.Default);
        if (oldAccount != null)
            oldAccount.Default = false;
        bankAccount.Default = true;

        Result = await organizationManager.UpdateAsync(data);
        return Page();
    }
}