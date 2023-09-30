using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.People.Pages.Detail;

public class BankAccountModel : PageModel
{
    private readonly NaturalPersonManager personManager;

    public BankAccountModel(NaturalPersonManager personManager)
    {
        this.personManager = personManager;
    }

    public NaturalPerson Person { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "{0}是必需的")]
    [MaxLength(50)]
    public string AccountNumber { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "{0}是必需的")]
    [MaxLength(150)]
    public string AccountName { get; set; } = default!;

    [BindProperty]
    [Required(ErrorMessage = "{0}是必需的")]
    [MaxLength(150)]
    public string BankName { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await this.personManager.FindByIdAsync(this.Id.ToString());
        if (result == null)
            return this.NotFound();
        this.Person = result;
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddBankAccountAsync()
    {
        var person = await this.personManager.FindByIdAsync(this.Id.ToString());
        if (person == null)
            return this.NotFound();
        this.Person = person;

        if (this.Person.BankAccounts.Any(p => p.AccountNumber == this.AccountNumber))
        {
            this.ModelState.AddModelError(nameof(this.AccountNumber), "此账号已存在。");
            return this.Page();
        }

        this.Person.BankAccounts.Add(new PersonBankAccount(this.AccountNumber.Trim(), this.AccountName.Trim(), this.BankName.Trim()));

        await this.personManager.UpdateAsync(this.Person);
        return this.Page();
    }

    public async Task<IActionResult> OnPostRemoveBankAccountAsync()
    {
        var person = await this.personManager.FindByIdAsync(this.Id.ToString());
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
