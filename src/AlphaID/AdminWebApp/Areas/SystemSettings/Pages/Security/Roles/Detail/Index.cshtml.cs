using AdminWebApp.Domain.Security;
using AlphaIdPlatform.Admin;
using Microsoft.AspNetCore.Mvc;

namespace AdminWebApp.Areas.SystemSettings.Pages.Security.Roles.Detail
{
    public class IndexModel(UserInRoleManager manager) : PageModel
    {
        public Role Role { get; set; } = null!;

        public IEnumerable<UserInRole> UserInrRoles { get; set; } = [];

        [TempData]
        public string? ResultMessage { get; set; }

        public IActionResult OnGet(string name)
        {
            Role? role = RoleConstants.Roles.FirstOrDefault(r => r.Name == name);
            if (role == null)
                return NotFound();
            Role = role;

            UserInrRoles = manager.GetUserInRoles(role.Name);

            return Page();
        }

        public async Task<IActionResult> OnPostRemove(string name, string userId)
        {
            await manager.RemoveRole(userId, name);
            ResultMessage = "User removed from role successfully.";
            return RedirectToPage(new { name });
        }
    }
}
