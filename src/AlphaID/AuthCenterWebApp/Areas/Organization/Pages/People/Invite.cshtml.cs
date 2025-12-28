using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Organizational;
using System.ComponentModel.DataAnnotations;

namespace AuthCenterWebApp.Areas.Organization.Pages.People;

public class InviteModel(
    UserManager<NaturalPerson> applicationUserManager,
    JoinOrganizationInvitationManager joinOrganizationInvitationManager,
    IOrganizationStore organizationStore) : PageModel
{
    [BindProperty]
    [Display(Name = "Invitee")]
    public string Invitee { get; set; } = null!;

    public OrganizationOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        var organization = organizationStore.Organizations.FirstOrDefault(o => o.Name == anchor);
        if (organization == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        var organization = organizationStore.Organizations.FirstOrDefault(o => o.Name == anchor);
        if (organization == null)
            return NotFound();
        NaturalPerson? person = await applicationUserManager.FindByNameAsync(Invitee);
        if (person == null)
            ModelState.AddModelError(nameof(Invitee), "Cannot find person.");

        if (!ModelState.IsValid)
            return Page();

        Result = await joinOrganizationInvitationManager.InviteMemberAsync(organization, person!, User.DisplayName());
        return Page();
    }

    public IActionResult OnGetFindPerson(string term)
    {
        IQueryable<FindPersonModel> searchResults = applicationUserManager.Users.Where(p =>
                p.UserName!.StartsWith(term) || p.Email!.StartsWith(term) || p.Name!.StartsWith(term))
            .Select(p => new FindPersonModel { UserName = p.UserName!, Name = p.Name });
        return new JsonResult(searchResults);
    }

    public class FindPersonModel
    {
        public string UserName { get; set; } = null!;

        public string? Name { get; set; }
    }
}