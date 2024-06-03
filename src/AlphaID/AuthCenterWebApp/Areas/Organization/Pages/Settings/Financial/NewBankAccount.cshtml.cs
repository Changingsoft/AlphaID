using System.ComponentModel.DataAnnotations;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class NewBankAccountModel(
    OrganizationManager organizationManager,
    OrganizationBankAccountManager bankAccountManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out GenericOrganization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        Result = await bankAccountManager.AddAsync(organization, Input.AccountNumber, Input.AccountName,
            Input.BankName, Input.Usage, Input.SetDefault);

        if (!Result.Succeeded)
            return Page();

        return RedirectToPage("Index", new { anchor });
    }

    public class InputModel
    {
        [Display(Name = "Account number")]
        [Required(ErrorMessage = "Validate_Required")]
        public string AccountNumber { get; set; } = default!;

        [Display(Name = "Account name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string AccountName { get; set; } = default!;

        [Display(Name = "Bank name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string BankName { get; set; } = default!;

        [Display(Name = "Usage")]
        public string? Usage { get; set; }

        [Display(Name = "Set default")]
        public bool SetDefault { get; set; } = false;
    }
}