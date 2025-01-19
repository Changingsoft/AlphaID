using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Subjects;
using IdSubjects;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.OrganizationManagement.Pages.Detail.Financial;

public class NewBankAccountModel(OrganizationManager organizationManager) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public async Task<IActionResult> OnGetAsync(string anchor)
    {
        Organization? org = await organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        Organization? org = await organizationManager.FindByIdAsync(anchor);
        if (org == null)
            return NotFound();

        if (!ModelState.IsValid)
            return Page();

        org.BankAccounts.Add(new OrganizationBankAccount()
        {
            AccountName = Input.AccountName,
            AccountNumber = Input.AccountNumber,
            BankName = Input.BankName,
            Usage = Input.Usage,
        });
        //todo 没有使用Input.SetDefault

        Result = await organizationManager.UpdateAsync(org);

        if (!Result.Succeeded)
            return Page();

        return RedirectToPage("Index", new { anchor });
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        public string AccountNumber { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Validate_Required")]
        public string BankName { get; set; } = null!;

        public string? Usage { get; set; }

        public bool SetDefault { get; set; } = false;
    }
}