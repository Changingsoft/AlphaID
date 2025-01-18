using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class NewBankAccountModel(
    OrganizationManager organizationManager,
    OrganizationBankAccountManager bankAccountManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
            return RedirectToPage("/Who", new { anchor });
        if (organization == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out AlphaIdPlatform.Subjects.Organization? organization))
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
        public string AccountNumber { get; set; } = null!;

        [Display(Name = "Account name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string AccountName { get; set; } = null!;

        [Display(Name = "Bank name")]
        [Required(ErrorMessage = "Validate_Required")]
        public string BankName { get; set; } = null!;

        [Display(Name = "Usage")]
        public string? Usage { get; set; }

        [Display(Name = "Set default")]
        public bool SetDefault { get; set; } = false;
    }
}