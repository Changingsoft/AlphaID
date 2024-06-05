using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class IndexModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager)
    : PageModel
{
    public GenericOrganization Data { get; set; } = default!;

    public IEnumerable<OrganizationBankAccount> BankAccounts { get; set; } = [];

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();
        Data = organization;
        BankAccounts = bankAccountManager.GetBankAccounts(Data);
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string accountNumber)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();
        Data = organization;
        BankAccounts = bankAccountManager.GetBankAccounts(Data);

        OrganizationBankAccount? bankAccount = BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null) return Page();

        Result = await bankAccountManager.RemoveAsync(bankAccount);
        if (Result.Succeeded)
            BankAccounts = bankAccountManager.GetBankAccounts(Data);
        return Page();
    }

    public async Task<IActionResult> OnPostSetDefaultAsync(string anchor, string accountNumber)
    {
        GenericOrganization? data = await organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return NotFound();
        Data = data;
        BankAccounts = bankAccountManager.GetBankAccounts(data);

        OrganizationBankAccount? bankAccount = BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null) return Page();

        Result = await bankAccountManager.SetDefault(bankAccount);
        if (Result.Succeeded)
            BankAccounts = bankAccountManager.GetBankAccounts(data);
        return Page();
    }
}