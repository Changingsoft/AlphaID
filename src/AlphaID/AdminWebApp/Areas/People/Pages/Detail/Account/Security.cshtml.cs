using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Transactions;

namespace AdminWebApp.Areas.People.Pages.Detail.Account;

public class SecurityModel : PageModel
{
    private readonly NaturalPersonManager manager;

    public SecurityModel(NaturalPersonManager manager)
    {
        this.manager = manager;
    }

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public NaturalPerson Data { get; set; } = default!;

    public string OperationMessage = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var person = await this.manager.FindByIdAsync(this.Anchor);
        if (person == null) { return this.NotFound(); }

        this.Data = person;
        this.Input = new InputModel
        {
            TwoFactorEnabled = this.Data.TwoFactorEnabled,
            LockoutEnabled = this.Data.LockoutEnabled,
        };
        return this.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var person = await this.manager.FindByIdAsync(this.Anchor);
        if (person == null) { return this.NotFound(); }

        this.Data = person;

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await this.manager.SetTwoFactorEnabledAsync(this.Data, this.Input.TwoFactorEnabled);
        await this.manager.SetLockoutEnabledAsync(this.Data, this.Input.LockoutEnabled);
        trans.Complete();
        this.OperationMessage = "ÒÑ¸üÐÂ";
        return this.Page();
    }

    public class InputModel
    {
        [Display(Name = "Two-factor enabled")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Lockout enabled")]
        public bool LockoutEnabled { get; set; }


    }
}
