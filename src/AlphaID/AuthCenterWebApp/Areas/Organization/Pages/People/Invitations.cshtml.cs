using AlphaIDPlatform.Security;
using IDSubjects;
using IDSubjects.Invitations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class InvitationsModel : PageModel
    {
        private readonly NaturalPersonManager naturalPersonManager;
        private readonly JoinOrganizationInvitationManager joinOrganizationInvitationManager;
        private readonly OrganizationManager organizationManager;

        public InvitationsModel(NaturalPersonManager naturalPersonManager, JoinOrganizationInvitationManager joinOrganizationInvitationManager, OrganizationManager organizationManager)
        {
            this.naturalPersonManager = naturalPersonManager;
            this.joinOrganizationInvitationManager = joinOrganizationInvitationManager;
            this.organizationManager = organizationManager;
        }

        [BindProperty]
        [Display(Name = "Invitee")]
        public string Invitee { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("../Who", new { anchor });
            if (organization == null)
                return this.NotFound();
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!this.organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("../Who", new { anchor });
            if (organization == null)
                return this.NotFound();
            var person = await this.naturalPersonManager.FindByNameAsync(this.Invitee);
            if (person == null)
                this.ModelState.AddModelError(nameof(this.Invitee), "Cannot find person.");

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await this.joinOrganizationInvitationManager.InviteMemberAsync(organization, person!, this.User.DisplayName() ?? "");
            return this.Page();
        }

        public IActionResult OnGetFindPerson(string term)
        {
            var searchResults = this.naturalPersonManager.Users.Where(p => p.UserName.StartsWith(term) || p.Email!.StartsWith(term) || p.PersonName.FullName.StartsWith(term))
                .Select(p => new FindPersonModel() { UserName = p.UserName, Name = p.PersonName.FullName });
            return new JsonResult(searchResults);
        }

        public class FindPersonModel
        {
            public string UserName { get; set; } = default!;

            public string Name { get; set; } = default!;
        }
    }
}
