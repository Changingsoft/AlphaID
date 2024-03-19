using AlphaIdPlatform.Security;
using IdSubjects;
using IdSubjects.Invitations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.People
{
    public class InvitationsModel(NaturalPersonManager naturalPersonManager, JoinOrganizationInvitationManager joinOrganizationInvitationManager, OrganizationManager organizationManager) : PageModel
    {
        [BindProperty]
        [Display(Name = "Invitee")]
        public string Invitee { get; set; } = default!;

        public IdOperationResult? Result { get; set; }

        public IActionResult OnGet(string anchor)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("../Who", new { anchor });
            if (organization == null)
                return this.NotFound();
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string anchor)
        {
            if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out var organization))
                return this.RedirectToPage("../Who", new { anchor });
            if (organization == null)
                return this.NotFound();
            var person = await naturalPersonManager.FindByNameAsync(this.Invitee);
            if (person == null)
                this.ModelState.AddModelError(nameof(this.Invitee), "Cannot find person.");

            if (!this.ModelState.IsValid)
                return this.Page();

            this.Result = await joinOrganizationInvitationManager.InviteMemberAsync(organization, person!, this.User.DisplayName() ?? "");
            return this.Page();
        }

        public IActionResult OnGetFindPerson(string term)
        {
            var searchResults = naturalPersonManager.Users.Where(p => p.UserName.StartsWith(term) || p.Email!.StartsWith(term) || p.PersonName.FullName.StartsWith(term))
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
