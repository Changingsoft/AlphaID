using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial
{
    public class NewBankAccountModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager) : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await bankAccountManager.AddAsync(organization, this.Input.AccountNumber, this.Input.AccountName,
                this.Input.BankName, this.Input.Usage, this.Input.SetDefault);

            if (!this.Result.Succeeded)
                return this.Page();

            return this.RedirectToPage("Index", new { anchor });
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
}
