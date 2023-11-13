using IDSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Financial
{
    public class NewBankAccountModel : PageModel
    {
        private OrganizationManager organizationManager;
        OrganizationBankAccountManager bankAccountManager;

        public NewBankAccountModel(OrganizationManager organizationManager, OrganizationBankAccountManager bankAccountManager)
        {
            this.organizationManager = organizationManager;
            this.bankAccountManager = bankAccountManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await this.bankAccountManager.AddAsync(organization, this.Input.AccountNumber, this.Input.AccountName,
                this.Input.BankName, this.Input.Usage, this.Input.SetDefault);

            if (!this.Result.Succeeded)
                return this.Page();

            return this.RedirectToPage("Index", new { anchor });
        }

        public class InputModel
        {
            public string AccountNumber { get; set; } = default!;

            public string AccountName { get; set; } = default!;

            public string BankName { get; set; } = default!;

            public string? Usage { get; set; }

            public bool SetDefault { get; set; } = false;
        }
    }
}
