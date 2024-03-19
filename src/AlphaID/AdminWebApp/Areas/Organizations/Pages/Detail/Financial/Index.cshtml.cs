using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.Organizations.Pages.Detail.Financial;

public class IndexModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager) : PageModel
{
    public GenericOrganization Data { get; set; } = default!;

    public IEnumerable<OrganizationBankAccount> BankAccounts { get; set; } = [];

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        var data = await organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        this.BankAccounts = bankAccountManager.GetBankAccounts(data);
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string accountNumber)
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

        this.Result = await bankAccountManager.RemoveAsync(bankAccount);
        if(this.Result.Succeeded)
            this.BankAccounts = bankAccountManager.GetBankAccounts(data);
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
