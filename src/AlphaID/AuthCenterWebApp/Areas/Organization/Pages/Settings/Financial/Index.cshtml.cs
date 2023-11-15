using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class IndexModel : PageModel
{
    private readonly OrganizationManager organizationManager;
    readonly OrganizationBankAccountManager bankAccountManager;
    public IndexModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager)
    {
        this.organizationManager = organizationManager;
        this.bankAccountManager = bankAccountManager;
    }

    public GenericOrganization Data { get; set; } = default!;

    public IEnumerable<OrganizationBankAccount> BankAccounts { get; set; } = Enumerable.Empty<OrganizationBankAccount>();

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
            return this.RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return this.NotFound();
        this.Data = organization;
        this.BankAccounts = this.bankAccountManager.GetBankAccounts(this.Data);
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveAsync(string anchor, string accountNumber)
    {
        if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
            return this.RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return this.NotFound();
        this.Data = organization;
        this.BankAccounts = this.bankAccountManager.GetBankAccounts(this.Data);

        var bankAccount = this.BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null)
        {
            return this.Page();
        }

        this.Result = await this.bankAccountManager.RemoveAsync(bankAccount);
        if(this.Result.Succeeded)
            this.BankAccounts = this.bankAccountManager.GetBankAccounts(this.Data);
        return this.Page();
    }

    public async Task<IActionResult> OnPostSetDefaultAsync(string anchor, string accountNumber)
    {
        var data = await this.organizationManager.FindByIdAsync(anchor);
        if (data == null)
            return this.NotFound();
        this.Data = data;
        this.BankAccounts = this.bankAccountManager.GetBankAccounts(data);

        var bankAccount = this.BankAccounts.FirstOrDefault(b => b.AccountNumber == accountNumber);
        if (bankAccount == null)
        {
            return this.Page();
        }

        this.Result = await this.bankAccountManager.SetDefault(bankAccount);
        if (this.Result.Succeeded)
            this.BankAccounts = this.bankAccountManager.GetBankAccounts(data);
        return this.Page();
    }

}
