using System.ComponentModel.DataAnnotations;
using IdSubjects;
using IdSubjects.Payments;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

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
        NaturalPerson? result = await personManager.FindByIdAsync(Anchor);
        if (result == null)
            return NotFound();
        Person = result;
        BankAccounts = bankAccountManager.GetBankAccounts(Person);
        return Page();
    }

    public async Task<IActionResult> OnPostAddBankAccountAsync()
    {
        NaturalPerson? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        BankAccounts = bankAccountManager.GetBankAccounts(person);

        if (BankAccounts.Any(p => p.AccountNumber == AccountNumber))
            ModelState.AddModelError(nameof(AccountNumber), "¥À’À∫≈“—¥Ê‘⁄°£");
        if (!ModelState.IsValid)
            return Page();

        Result = await bankAccountManager.AddBankAccountAsync(person,
            new BankAccountInfo(AccountNumber, AccountName, BankName));
        if (Result.Succeeded)
            BankAccounts = bankAccountManager.GetBankAccounts(person);
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveBankAccountAsync(string accountNumber)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        BankAccounts = bankAccountManager.GetBankAccounts(person);

        Result = await bankAccountManager.RemoveBankAccountAsync(person, accountNumber);
        if (Result.Succeeded) BankAccounts = bankAccountManager.GetBankAccounts(person);
        return Page();
    }
}