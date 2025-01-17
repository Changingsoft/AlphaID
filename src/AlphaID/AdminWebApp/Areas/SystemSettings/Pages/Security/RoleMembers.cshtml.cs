using System.ComponentModel.DataAnnotations;
using AlphaIdPlatform.Admin;
using AlphaIdPlatform.Identity;
using IdSubjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security;

public class RoleMembersModel(UserInRoleManager userInRoleManager, UserManager<NaturalPerson> personManager) : PageModel
{
    public IEnumerable<UserInRole>? RoleMembers { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = null!;

    public IActionResult OnGet(string? role)
    {
        if (role == null)
            return Page();

        RoleMembers = userInRoleManager.GetUserInRoles(role);
        return Page();
    }

    public async Task<IActionResult> OnPostAddMemberAsync(string role)
    {
        NaturalPerson person = await personManager.FindByNameAsync(Input.UserName) ??
                               throw new InvalidOperationException("User cannot found.");
        await userInRoleManager.AddRole(person.Id, role);
        Input = null!;
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(string role, string personId)
    {
        await userInRoleManager.RemoveRole(personId, role);
        return RedirectToPage();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Validate_Required")]
        public string UserName { get; set; } = null!;

        public string PhoneticSearchHint { get; set; } = null!;
    }
}