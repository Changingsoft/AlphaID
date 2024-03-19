using IdSubjects;
using IdSubjects.Payments;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class PaymentsModel(NaturalPersonManager personManager, PersonBankAccountManager bankAccountManager) : PageModel
{
    public NaturalPerson Person { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Account number")]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Account name")]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(150)]
    public string AccountName { get; set; } = default!;

    [BindProperty]
    [Display(Name = "Bank name")]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(150)]
    public string BankName { get; set; } = default!;

    public IEnumerable<PersonBankAccount> BankAccounts { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await personManager.FindByIdAsync(this.Anchor);
        if (result == null)
            return this.NotFound();
        this.Person = result;
        this.BankAccounts = bankAccountManager.GetBankAccounts(this.Person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddBankAccountAsync()
    {
        var person = await personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.BankAccounts = bankAccountManager.GetBankAccounts(person);

        if (this.BankAccounts.Any(p => p.AccountNumber == this.AccountNumber))
            this.ModelState.AddModelError(nameof(this.AccountNumber), "¥À’À∫≈“—¥Ê‘⁄°£");
        if (!this.ModelState.IsValid)
            return this.Page();

        this.Result = await bankAccountManager.AddBankAccountAsync(person, new BankAccountInfo(this.AccountNumber, this.AccountName, this.BankName));
        if(this.Result.Succeeded)
            this.BankAccounts = bankAccountManager.GetBankAccounts(person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveBankAccountAsync(string accountNumber)
    {
        var person = await personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;
        this.BankAccounts = bankAccountManager.GetBankAccounts(person);

        this.Result = await bankAccountManager.RemoveBankAccountAsync(person, accountNumber);
        if (this.Result.Succeeded)
        {
            this.BankAccounts = bankAccountManager.GetBankAccounts(person);
        }
        return this.Page();
    }
}
