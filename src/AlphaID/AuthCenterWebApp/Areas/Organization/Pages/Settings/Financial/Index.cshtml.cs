using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class IndexModel(OrganizationManager organizationManager, IOrganizationStore store) : PageModel
{
    public Organizational.Organization Data { get; set; } = null!;

    public ICollection<OrganizationBankAccount> BankAccounts { get; set; } = [];

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Data = organization;
        BankAccounts = organization.BankAccounts;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string accountNumber)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();
        Data = organization;
        BankAccounts = organization.BankAccounts;

        OrganizationBankAccount? bankAccount = BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null) return Page();

        BankAccounts.Remove(bankAccount);

        Result = await store.UpdateAsync(organization);
        return Page();
    }

    public async Task<IActionResult> OnPostSetDefaultAsync(string anchor, string accountNumber)
    {
        Organizational.Organization? data = await store.FindByIdAsync(anchor);
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

        Result = await store.UpdateAsync(data);
        return Page();
    }
}