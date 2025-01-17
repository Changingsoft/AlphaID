using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Identity;
using AlphaIdPlatform.Invitations;
using AlphaIdPlatform.Security;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthCenterWebApp.Areas.Organization.Pages.People;

public class InviteModel(
    UserManager<NaturalPerson> applicationUserManager,
    JoinOrganizationInvitationManager joinOrganizationInvitationManager,
    OrganizationManager organizationManager) : PageModel
{
    [BindProperty]
    [Display(Name = "Invitee")]
    public string Invitee { get; set; } = null!;

    public IdOperationResult? Result { get; set; }

    public IActionResult OnGet(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("../Who", new { anchor });
        if (organization == null)
            return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string anchor)
    {
        if (!organizationManager.TryGetSingleOrDefaultOrganization(anchor, out IdSubjects.Organization? organization))
            return RedirectToPage("../Who", new { anchor });
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
                p.UserName.StartsWith(term) || p.Email!.StartsWith(term) || p.PersonName.FullName.StartsWith(term))
            .Select(p => new FindPersonModel { UserName = p.UserName, Name = p.PersonName.FullName });
        return new JsonResult(searchResults);
    }

    public class FindPersonModel
    {
        public string UserName { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}