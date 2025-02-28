using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial;

public class NewBankAccountModel(OrganizationManager organizationManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGet(string anchor)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        var organization = await organizationManager.FindByNameAsync(anchor);
        if (organization == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        organization.BankAccounts.Add(new OrganizationBankAccount()
        {
            AccountName = Input.AccountName,
            AccountNumber = Input.AccountNumber,
            BankName = Input.BankName,
            Usage = Input.Usage,
        });
        //todo 没有使用Input.SetDefault

        Result = await organizationManager.UpdateAsync(organization);

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