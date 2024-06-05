using System.ComponentModel.DataAnnotations;
using System.Transactions;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.People.Pages.Detail.Account;

public class SecurityModel(NaturalPersonManager manager) : PageModel
{
    public string OperationMessage = default!;

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = default!;

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public NaturalPerson Data { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        NaturalPerson? person = await manager.FindByIdAsync(Anchor);
        if (person == null) return NotFound();

        Data = person;
        Input = new InputModel
        {
            TwoFactorEnabled = Data.TwoFactorEnabled,
            LockoutEnabled = Data.LockoutEnabled
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        NaturalPerson? person = await manager.FindByIdAsync(Anchor);
        if (person == null) return NotFound();

        Data = person;

        using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await manager.SetTwoFactorEnabledAsync(Data, Input.TwoFactorEnabled);
        await manager.SetLockoutEnabledAsync(Data, Input.LockoutEnabled);
        trans.Complete();
        OperationMessage = "ÒÑ¸üÐÂ";
        return Page();
    }

    public class InputModel
    {
        [Display(Name = "Two-factor enabled")]
        public bool TwoFactorEnabled { get; set; }

        [Display(Name = "Lockout enabled")]
        public bool LockoutEnabled { get; set; }
    }
}