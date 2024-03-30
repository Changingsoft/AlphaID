using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.Organizations.Pages.Detail.Financial
{
    public class NewBankAccountModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public async Task<IActionResult> OnGetAsync(string anchor)
        {
            var org = await organizationManager.FindByIdAsync(anchor);
            if (org == null)
                return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            var org = await organizationManager.FindByIdAsync(anchor);
            if (org == null)
                return NotFound();

            if (!ModelState.IsValid)
                return Page();

            Result = await bankAccountManager.AddAsync(org, Input.AccountNumber, Input.AccountName,
                Input.BankName, Input.Usage, Input.SetDefault);

            if (!Result.Succeeded)
                return Page();

            return RedirectToPage("Index", new { anchor });
        }

        public class InputModel
        {
        [Required(ErrorMessage = "Validate_Required")]
            public string AccountNumber { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
            public string AccountName { get; set; } = default!;

        [Required(ErrorMessage = "Validate_Required")]
            public string BankName { get; set; } = default!;

            public string? Usage { get; set; }

            public bool SetDefault { get; set; } = false;
        }
    }
}
