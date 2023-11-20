using IdSubjects;
using IdSubjects.Payments;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class PaymentsModel : PageModel
{
    private readonly NaturalPersonManager personManager;
    readonly PersonBankAccountManager bankAccountManager;

    public PaymentsModel(NaturalPersonManager personManager, PersonBankAccountManager bankAccountManager)
    {
        this.personManager = personManager;
        this.bankAccountManager = bankAccountManager;
    }

    public NaturalPerson Person { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(150)]
    public string AccountName { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(150)]
    public string BankName { get; set; } = default!;

    public IEnumerable<PersonBankAccount> BankAccounts { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await this.personManager.FindByIdAsync(this.Anchor);
        if (result == null)
            return this.NotFound();
        this.Person = result;
        this.BankAccounts = this.bankAccountManager.GetBankAccounts(this.Person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddBankAccountAsync()
    {
        var person = await this.personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.BankAccounts = this.bankAccountManager.GetBankAccounts(person);

        if (this.BankAccounts.Any(p => p.AccountNumber == this.AccountNumber))
            this.ModelState.AddModelError(nameof(this.AccountNumber), "¥À’À∫≈“—¥Ê‘⁄°£");
        if (!this.ModelState.IsValid)
            return this.Page();

        this.Result = await this.bankAccountManager.AddBankAccountAsync(person, new BankAccountInfo(this.AccountNumber, this.AccountName, this.BankName));
        if(this.Result.Succeeded)
            this.BankAccounts = this.bankAccountManager.GetBankAccounts(person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveBankAccountAsync(string accountNumber)
    {
        var person = await this.personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.BankAccounts = this.bankAccountManager.GetBankAccounts(person);

        this.Result = await this.bankAccountManager.RemoveBankAccountAsync(person, accountNumber);
        if (this.Result.Succeeded)
        {
            this.BankAccounts = this.bankAccountManager.GetBankAccounts(person);
        }
        return this.Page();
    }
}
