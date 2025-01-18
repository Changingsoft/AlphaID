using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail;

public class PaymentsModel(UserManager<NaturalPerson> personManager) : PageModel
{
    public NaturalPerson Person { get; set; } = null!;

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

    public IEnumerable<NaturalPersonBankAccount> BankAccounts { get; set; } = null!;

    public IdentityResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? result = await personManager.FindByIdAsync(Anchor);
        if (result == null)
            return NotFound();
        Person = result;
        BankAccounts = result.BankAccounts;
        return Page();
    }

    public async Task<IActionResult> OnPostAddBankAccountAsync()
    {
        NaturalPerson? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        BankAccounts = person.BankAccounts;

        if (BankAccounts.Any(p => p.AccountNumber == AccountNumber))
            ModelState.AddModelError(nameof(AccountNumber), "此账号已存在。");
        if (!ModelState.IsValid)
            return Page();

        person.BankAccounts.Add(new NaturalPersonBankAccount()
        {
            AccountNumber = AccountNumber,
            AccountName = AccountName,
            BankName = BankName,
        });

        Result = await personManager.UpdateAsync(person);
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveBankAccountAsync(string accountNumber)
    {
        NaturalPerson? person = await personManager.FindByIdAsync(Anchor);
        if (person == null)
            return NotFound();
        Person = person;
        BankAccounts = person.BankAccounts;

        var account = person.BankAccounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        if (account != null)
        {
            person.BankAccounts.Remove(account);
            Result = await personManager.UpdateAsync(person);
        }
        return Page();
    }
}