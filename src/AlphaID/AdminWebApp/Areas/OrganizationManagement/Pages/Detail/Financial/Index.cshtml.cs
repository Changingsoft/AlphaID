using Microsoft.AspNetCore.Mvc;
using Organizational;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail.Financial;

public class IndexModel(OrganizationManager organizationManager)
    : PageModel
{
    public Organization Data { get; set; } = null!;

    public ICollection<OrganizationBankAccount> BankAccounts { get; set; } = [];

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? data = await organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();
        Data = data;
        BankAccounts = data.BankAccounts;
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string accountNumber)
    {
        Organization? data = await organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();
        Data = data;
        BankAccounts = data.BankAccounts;

        OrganizationBankAccount? bankAccount = BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null) return Page();

        BankAccounts.Remove(bankAccount);

        Result = await organizationManager.UpdateAsync(data);
        return Page();
    }

    public async Task<IActionResult> OnPostSetDefaultAsync(string anchor, string accountNumber)
    {
        Organization? data = await organizationManager.FindByIdAsync(anchor);
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

        await organizationManager.UpdateAsync(data);
        return Page();
    }
}