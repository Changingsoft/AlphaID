using AlphaIDPlatform.Security;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security;

public class RoleMembersModel : PageModel
{
    private readonly UserInRoleManager userInRoleManager;

    public RoleMembersModel(UserInRoleManager userInRoleManager)
    {
        this.userInRoleManager = userInRoleManager;
    }

    public IEnumerable<UserInRole>? RoleMembers { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Role { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public IActionResult OnGet(string? role)
    {
        if (role == null)
            return this.Page();

        this.RoleMembers = this.userInRoleManager.GetUserInRoles(role);
        return this.Page();
    }

    public async Task<IActionResult> OnPostAddMemberAsync(string role)
    {
        if (this.userInRoleManager.GetRoles(this.Input.PersonId).Any(p => p == role))
        {
            return this.Page();
        }
        var userInRole = new UserInRole()
        {
            UserId = this.Input.PersonId,
            RoleName = role,
            UserName = this.Input.UserName,
            UserSearchHint = this.Input.PhoneticSearchHint,
        };
        await this.userInRoleManager.AddRole(this.Input.PersonId, role, this.Input.UserName, this.Input.PhoneticSearchHint);
        this.Input = default!;
        return this.RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(string role, string personId)
    {
        await this.userInRoleManager.RemoveRole(personId, role);
        return this.RedirectToPage();
    }

    public class InputModel
    {
        [Required(ErrorMessage = "{0}是必需的。")]
        public string PersonId { get; set; } = default!;

        [Required(ErrorMessage = "{0}是必需的。")]
        public string UserName { get; set; } = default!;

        public string PhoneticSearchHint { get; set; } = default!;
    }
}
