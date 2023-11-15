using IdSubjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.Settings.Identifiers
{
    public class IndexModel : PageModel
    {
        private readonly OrganizationManager organizationManager;
        private readonly OrganizationIdentifierManager identifierManager;

        public IndexModel(OrganizationManager organizationManager, OrganizationIdentifierManager identifierManager)
        {
            this.organizationManager = organizationManager;
            this.identifierManager = identifierManager;
        }

        public IEnumerable<OrganizationIdentifier> Identifiers { get; set; } = Enumerable.Empty<OrganizationIdentifier>();

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();
            this.Identifiers = this.identifierManager.GetIdentifiers(organization);
            return this.Page();
        }

        public async Task<IActionResult> OnPostRemove(string anchor, string idKey)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("/Who", new { anchor });
            if (organization == null)
                return this.NotFound();
            this.Identifiers = this.identifierManager.GetIdentifiers(organization);

            var keyPart = idKey.Split('|');
            var type = Enum.Parse<OrganizationIdentifierType>(keyPart[0]);
            var identifier = this.Identifiers.FirstOrDefault(i => i.Type == type && i.Value == keyPart[1]);
            if (identifier == null)
                return this.Page();

            this.Result = await this.identifierManager.RemoveIdentifierAsync(identifier);
            if (this.Result.Succeeded)
            {
                this.Identifiers = this.identifierManager.GetIdentifiers(organization);
            }

            return this.Page();
        }
    }
}
