using System.ComponentModel.DataAnnotations;
using IdSubjects;
using IdSubjects.Payments;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class PaymentsModel(ApplicationUserManager<ApplicationUser> personManager, ApplicationUserBankAccountManager bankAccountManager) : PageModel
{
    public ApplicationUser Person { get; set; } = null!;

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Account number")]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Account name")]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(150)]
    public string AccountName { get; set; } = null!;

    [BindProperty]
    [Display(Name = "Bank name")]
    [Required(ErrorMessage = "Validate_Required")]
    [MaxLength(150)]
    public string BankName { get; set; } = null!;

    public IEnumerable<ApplicationUserBankAccount> BankAccounts { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        ApplicationUser? result = await personManager.FindByIdAsync(Anchor);
        if (result == null)
            return NotFound();
        Person = result;
        BankAccounts = bankAccountManager.GetBankAccounts(Person);
        return Page();
    }

    public async Task<IActionResult> OnPostAddBankAccountAsync()
    {
        ApplicationUser? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        BankAccounts = bankAccountManager.GetBankAccounts(person);

        if (BankAccounts.Any(p => p.AccountNumber == AccountNumber))
            ModelState.AddModelError(nameof(AccountNumber), "此账号已存在。");
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
        ApplicationUser? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        BankAccounts = bankAccountManager.GetBankAccounts(person);

        Result = await bankAccountManager.RemoveBankAccountAsync(person, accountNumber);
        if (Result.Succeeded) BankAccounts = bankAccountManager.GetBankAccounts(person);
        return Page();
    }
}