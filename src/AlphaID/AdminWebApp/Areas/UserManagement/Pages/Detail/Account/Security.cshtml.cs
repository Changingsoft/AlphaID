using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AlphaIdPlatform.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.UserManagement.Pages.Detail.Account;

public class SecurityModel(UserManager<NaturalPerson> manager) : PageModel
{
    public string OperationMessage = null!;

    [BindProperty(SupportsGet = true)]
    public string Anchor { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public NaturalPerson Data { get; set; } = null!;

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
        OperationMessage = "已更新";
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