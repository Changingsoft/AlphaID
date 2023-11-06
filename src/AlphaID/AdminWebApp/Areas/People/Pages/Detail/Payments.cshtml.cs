using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class PaymentsModel : PageModel
{
    private readonly NaturalPersonManager personManager;

    public PaymentsModel(NaturalPersonManager personManager)
    {
        this.personManager = personManager;
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

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await this.personManager.FindByIdAsync(this.Anchor);
        if (result == null)
            return this.NotFound();
        this.Person = result;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddBankAccountAsync()
    {
        var person = await this.personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;

        if (this.Person.BankAccounts.Any(p => p.AccountNumber == this.AccountNumber))
        {
            this.ModelState.AddModelError(nameof(this.AccountNumber), "¥À’À∫≈“—¥Ê‘⁄°£");
            return this.Page();
        }

        this.Person.BankAccounts.Add(new PersonBankAccount(this.AccountNumber.Trim(), this.AccountName.Trim(), this.BankName.Trim()));

        await this.personManager.UpdateAsync(this.Person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveBankAccountAsync()
    {
        var person = await this.personManager.FindByIdAsync(this.Anchor);
        if (person == null)
            return this.NotFound();
        this.Person = person;

        var account = this.Person.BankAccounts.FirstOrDefault(p => p.AccountNumber == this.AccountNumber);
        if (account != null)
        {
            this.Person.BankAccounts.Remove(account);
            await this.personManager.UpdateAsync(this.Person);
        }
        return this.Page();
    }
}
