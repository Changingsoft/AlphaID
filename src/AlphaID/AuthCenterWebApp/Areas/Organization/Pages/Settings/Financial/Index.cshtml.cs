using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class IndexModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager) : PageModel
{
    public GenericOrganization Data { get; set; } = default!;

    public IEnumerable<OrganizationBankAccount> BankAccounts { get; set; } = [];

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
            return this.RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return this.NotFound();
        this.Data = organization;
        this.BankAccounts = bankAccountManager.GetBankAccounts(this.Data);
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string accountNumber)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
            return this.RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return this.NotFound();
        this.Data = organization;
        this.BankAccounts = bankAccountManager.GetBankAccounts(this.Data);

        var bankAccount = this.BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null)
        {
            return this.Page();
        }

        this.Result = await bankAccountManager.RemoveAsync(bankAccount);
        if(this.Result.Succeeded)
            this.BankAccounts = bankAccountManager.GetBankAccounts(this.Data);
        return this.Page();
    }

    public async Task<IActionResult> OnPostSetDefaultAsync(string anchor, string accountNumber)
    {
        var data = await organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        this.BankAccounts = bankAccountManager.GetBankAccounts(data);

        var bankAccount = this.BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null)
        {
            return this.Page();
        }

        this.Result = await bankAccountManager.SetDefault(bankAccount);
        if (this.Result.Succeeded)
            this.BankAccounts = bankAccountManager.GetBankAccounts(data);
        return this.Page();
    }

}
